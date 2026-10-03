namespace EcommerceDDD.Core.Infrastructure.Tests.OpenTelemetry;

public class MeterRegistrationTests
{
	private const string ServiceName = "EcommerceDDD.MeterProbe";

	[Fact]
	public void Observability_ShouldExportWolverineAndServiceMeters_AndIgnoreOthers()
	{
		// Given: the real registration, plus a reader that captures what the SDK exports
		var exported = new List<string>();
		var services = new ServiceCollection();
		services.AddOpenTelemetryObservability(ServiceName);
		services.ConfigureOpenTelemetryMeterProvider(builder =>
			builder.AddReader(new BaseExportingMetricReader(new CapturingExporter(exported))));

		using var provider = services.BuildServiceProvider();
		var meterProvider = provider.GetRequiredService<MeterProvider>();

		// Wolverine names its meter "Wolverine:" + ServiceName; a service names its own after the assembly
		using var wolverineMeter = new Meter($"Wolverine:{ServiceName}");
		using var serviceMeter = new Meter(ServiceName);
		using var unrelatedMeter = new Meter("Some.Other.Library");

		// When
		wolverineMeter.CreateCounter<long>("wolverine-probe").Add(1);
		serviceMeter.CreateCounter<long>("service-probe").Add(1);
		unrelatedMeter.CreateCounter<long>("unrelated-probe").Add(1);
		meterProvider.ForceFlush();

		// Then
		Assert.Contains("wolverine-probe", exported);
		Assert.Contains("service-probe", exported);
		Assert.DoesNotContain("unrelated-probe", exported);
	}

	[Fact]
	public async Task Wolverine_ShouldPublishItsInstruments_OnTheMeterNamedAfterTheService()
	{
		// Given
		var published = new List<(string Meter, string Instrument)>();
		using var listener = new MeterListener
		{
			InstrumentPublished = (instrument, _) =>
			{
				lock (published)
					published.Add((instrument.Meter.Name, instrument.Name));
			}
		};
		listener.Start();

		// When
		using var host = await Host.CreateDefaultBuilder()
			.UseWolverine(options =>
			{
				options.ServiceName = ServiceName;
				options.ApplicationAssembly = typeof(MeterRegistrationTests).Assembly;
				options.Discovery.DisableConventionalDiscovery();
			})
			.StartAsync();

		// Then: the names AddMeter("Wolverine:*") has to match
		var instruments = published
			.Where(p => p.Meter == $"Wolverine:{ServiceName}")
			.Select(p => p.Instrument)
			.ToList();

		Assert.Contains("wolverine-execution-failure", instruments);
		Assert.Contains("wolverine-dead-letter-queue", instruments);
		Assert.Contains("wolverine-messages-succeeded", instruments);
	}

	private sealed class CapturingExporter(List<string> exported) : BaseExporter<Metric>
	{
		public override ExportResult Export(in Batch<Metric> batch)
		{
			foreach (var metric in batch)
				exported.Add(metric.Name);

			return ExportResult.Success;
		}
	}
}
