
namespace EcommerceDDD.Core.Infrastructure.Tests.OpenTelemetry;

public record TraceProbe([property: Audit] Guid OrderId);

public static class TraceProbeHandler
{
	public static Activity? HandlerActivity;

	public static void Handle(TraceProbe probe) => HandlerActivity = Activity.Current;
}

public class TracePropagationTests : IDisposable
{
	private static readonly ActivitySource _testSource = new("EcommerceDDD.Tests");
	private readonly ActivityListener _listener;

	public TracePropagationTests()
	{
		_listener = new ActivityListener
		{
			ShouldListenTo = source => source.Name is ActivitySources.Wolverine or "EcommerceDDD.Tests",
			Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
		};
		ActivitySource.AddActivityListener(_listener);
	}

	[Fact]
	public async Task PublishedMessage_HandlerSpan_ShouldJoinPublisherTrace_AndCarryOrderIdTag()
	{
		// Given
		using var host = await Host.CreateDefaultBuilder()
			.UseWolverine(options =>
			{
				options.ApplicationAssembly = typeof(TracePropagationTests).Assembly;
				options.Discovery.DisableConventionalDiscovery().IncludeType(typeof(TraceProbeHandler));
			})
			.StartAsync();

		var orderId = Guid.NewGuid();
		TraceProbeHandler.HandlerActivity = null;

		// When
		using var publisherSpan = _testSource.StartActivity("place-order")!;
		var session = await host.TrackActivity()
			.Timeout(TimeSpan.FromSeconds(30))
			.PublishMessageAndWaitAsync(new TraceProbe(orderId));

		// Then: the envelope itself carries the trace (this is what crosses the Kafka hop)...
		var envelope = session.Sent.SingleEnvelope<TraceProbe>();
		Assert.Contains(publisherSpan.TraceId.ToHexString(), envelope.ParentId);

		// ...and the handler span continues it, tagged with the order id by [Audit]
		var handlerSpan = TraceProbeHandler.HandlerActivity;
		Assert.NotNull(handlerSpan);
		Assert.Equal(ActivitySources.Wolverine, handlerSpan.Source.Name);
		Assert.Equal(publisherSpan.TraceId, handlerSpan.TraceId);
		Assert.NotEqual(publisherSpan.SpanId, handlerSpan.SpanId);
		Assert.Equal(orderId.ToString(), handlerSpan.GetTagItem("order.id")?.ToString());
	}

	[Fact]
	public async Task ServerError_ShouldMarkSpanAsError_AndRecordException()
	{
		// Given
		using var span = _testSource.StartActivity("request")!;
		var context = new DefaultHttpContext();
		context.Response.Body = new MemoryStream();
		var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);

		// When
		await handler.TryHandleAsync(context, new InvalidOperationException("boom"), CancellationToken.None);

		// Then
		Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
		Assert.Equal(ActivityStatusCode.Error, span.Status);
		Assert.Contains(span.Events, e => e.Name == "exception");
	}

	[Fact]
	public async Task ClientError_ShouldLeaveSpanStatusUnset()
	{
		// Given
		using var span = _testSource.StartActivity("request")!;
		var context = new DefaultHttpContext();
		context.Response.Body = new MemoryStream();
		var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);

		// When
		await handler.TryHandleAsync(context, new DomainException("rule broken"), CancellationToken.None);

		// Then
		Assert.Equal(StatusCodes.Status422UnprocessableEntity, context.Response.StatusCode);
		Assert.Equal(ActivityStatusCode.Unset, span.Status);
		Assert.DoesNotContain(span.Events, e => e.Name == "exception");
	}

	public void Dispose() => _listener.Dispose();
}
