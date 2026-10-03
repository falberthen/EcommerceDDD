namespace EcommerceDDD.OrderProcessing.Tests.Infrastructure;

/// <summary>
/// Provides an OrderMetrics on its own meter factory and records its measurements.
/// </summary>
public sealed class OrderMetricsRecorder : IDisposable
{
	private readonly ServiceProvider _provider = new ServiceCollection().AddMetrics().BuildServiceProvider();
	private readonly MeterListener _listener = new();

	public OrderMetrics Metrics { get; }
	public List<(long Value, Dictionary<string, object?> Tags)> Measurements { get; } = [];

	public OrderMetricsRecorder()
	{
		var meterFactory = _provider.GetRequiredService<IMeterFactory>();

		// Only this factory's meters, so tests running in parallel don't mix.
		_listener.InstrumentPublished = (instrument, listener) =>
		{
			if (instrument.Meter.Scope == meterFactory)
				listener.EnableMeasurementEvents(instrument);
		};
		_listener.SetMeasurementEventCallback<long>((_, value, tags, _) =>
			Measurements.Add((value, tags.ToArray().ToDictionary(t => t.Key, t => t.Value))));
		_listener.Start();

		Metrics = new OrderMetrics(meterFactory);
	}

	public void Dispose()
	{
		_listener.Dispose();
		_provider.Dispose();
	}
}
