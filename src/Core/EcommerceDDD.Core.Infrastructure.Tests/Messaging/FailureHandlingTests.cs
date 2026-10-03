namespace EcommerceDDD.Core.Infrastructure.Tests.Messaging;

public record FailureProbe(Guid Id, string Outcome);

public static class FailureProbeHandler
{
	public static readonly ConcurrentDictionary<Guid, int> Executions = new();

	public static void Handle(FailureProbe probe)
	{
		Executions.AddOrUpdate(probe.Id, 1, (_, count) => count + 1);

		throw probe.Outcome switch
		{
			"broken rule" => new DomainException("rule broken"),
			"dependency unreachable" => new HttpRequestException("connection refused"),
			"write conflict" => new EventStreamUnexpectedMaxEventIdException("stream changed"),
			"duplicate insert" => new DocumentAlreadyExistsException(new Exception("unique index"), typeof(FailureProbe), probe.Id),
			_ => new InvalidOperationException("dependency down")
		};
	}
}

public class FailureHandlingTests(FailureHandlingTests.WolverineHost wolverine)
	: IClassFixture<FailureHandlingTests.WolverineHost>
{
	[Theory]
	[InlineData("dependency unreachable", typeof(HttpRequestException))]
	[InlineData("write conflict", typeof(EventStreamUnexpectedMaxEventIdException))]
	[InlineData("duplicate insert", typeof(DocumentAlreadyExistsException))]
	public async Task Invoked_TransientException_ShouldBeRetriedInPlace(string outcome, Type exception)
	{
		// Given
		var probe = new FailureProbe(Guid.NewGuid(), outcome);

		// When
		var invoke = () => _bus.InvokeAsync(probe);

		// Then: the first attempt and three retries
		await Assert.ThrowsAsync(exception, invoke);
		Assert.Equal(4, FailureProbeHandler.Executions[probe.Id]);
	}

	[Theory]
	[InlineData("broken rule", typeof(DomainException))]
	[InlineData("dependency down", typeof(InvalidOperationException))]
	public async Task Invoked_OtherException_ShouldReachTheCallerAtOnce(string outcome, Type exception)
	{
		// Given
		var probe = new FailureProbe(Guid.NewGuid(), outcome);

		// When
		var invoke = () => _bus.InvokeAsync(probe);

		// Then
		await Assert.ThrowsAsync(exception, invoke);
		Assert.Equal(1, FailureProbeHandler.Executions[probe.Id]);
	}

	[Fact]
	public async Task Published_BrokenRule_ShouldDeadLetter_WithoutRetrying()
	{
		// When
		var session = await _host.TrackActivity()
			.DoNotAssertOnExceptionsDetected()
			.PublishMessageAndWaitAsync(new FailureProbe(Guid.NewGuid(), "broken rule"));

		// Then
		var thrown = Assert.IsType<DomainException>(Assert.Single(session.AllExceptions()));
		Assert.Equal("rule broken", thrown.Message);
		Assert.Equal(1, session.MovedToErrorQueue.SingleEnvelope<FailureProbe>().Attempts);
	}

	[Fact]
	public async Task Published_OtherException_ShouldBeRetriedLater()
	{
		// Given
		var probe = new FailureProbe(Guid.NewGuid(), "dependency down");

		// When
		await _bus.PublishAsync(probe);

		// Then: the first retry is scheduled five seconds after the failure
		var deadline = DateTime.UtcNow.AddSeconds(15);
		while (FailureProbeHandler.Executions.GetValueOrDefault(probe.Id) < 2 && DateTime.UtcNow < deadline)
			await Task.Delay(100);

		Assert.True(FailureProbeHandler.Executions[probe.Id] >= 2);
	}

	public class WolverineHost : IAsyncLifetime
	{
		public IHost Host { get; private set; } = null!;

		public async Task InitializeAsync() =>
			Host = await Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
				.ConfigureAppConfiguration(configuration => configuration.AddInMemoryCollection(
					new Dictionary<string, string?> { ["TokenIssuerSettings:Authority"] = "http://localhost" }))
				.ConfigureServices((context, services) =>
					services.AddCoreInfrastructure(context.Configuration, options =>
					{
						options.ApplicationAssembly = typeof(FailureHandlingTests).Assembly;
						options.Discovery.DisableConventionalDiscovery().IncludeType(typeof(FailureProbeHandler));
					}))
				.StartAsync();

		public async Task DisposeAsync()
		{
			await Host.StopAsync();
			Host.Dispose();
		}
	}

	private readonly IHost _host = wolverine.Host;
	private readonly IMessageBus _bus = wolverine.Host.Services.GetRequiredService<IMessageBus>();
}
