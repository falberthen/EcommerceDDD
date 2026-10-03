namespace EcommerceDDD.Core.Infrastructure.Tests.Messaging;

public record ResultProbe(Guid Id, bool Fail);

public static class ResultProbeHandler
{
	public static readonly ConcurrentDictionary<Guid, int> Executions = new();

	public static Task<Result> HandleAsync(ResultProbe probe)
	{
		Executions.AddOrUpdate(probe.Id, 1, (_, count) => count + 1);
		return Task.FromResult(probe.Fail ? Result.Fail(new ValidationError("rule broken")) : Result.Ok());
	}
}

public class FailedResultTests(FailedResultTests.WolverineHost wolverine)
	: IClassFixture<FailedResultTests.WolverineHost>
{
	[Fact]
	public async Task Invoked_FailedResult_ShouldReachTheCaller()
	{
		var result = await _bus.InvokeAsync<Result>(new ResultProbe(Guid.NewGuid(), true));

		Assert.True(result.IsFailed);
	}

	[Fact]
	public async Task Published_FailedResult_ShouldDeadLetter_WithoutRetrying()
	{
		var session = await _host.TrackActivity()
			.DoNotAssertOnExceptionsDetected()
			.PublishMessageAndWaitAsync(new ResultProbe(Guid.NewGuid(), true));

		var thrown = Assert.IsType<FailedResultException>(Assert.Single(session.AllExceptions()));
		Assert.Equal("rule broken", thrown.Message);
		Assert.Equal(1, session.MovedToErrorQueue.SingleEnvelope<ResultProbe>().Attempts);
	}

	[Fact]
	public async Task Published_SuccessfulResult_ShouldComplete()
	{
		var session = await _host.TrackActivity()
			.PublishMessageAndWaitAsync(new ResultProbe(Guid.NewGuid(), false));

		Assert.Empty(session.AllExceptions());
		Assert.Empty(session.MovedToErrorQueue.Envelopes());
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
						options.ApplicationAssembly = typeof(FailedResultTests).Assembly;
						options.Discovery.DisableConventionalDiscovery().IncludeType(typeof(ResultProbeHandler));
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
