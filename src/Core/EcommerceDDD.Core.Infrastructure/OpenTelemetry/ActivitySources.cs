namespace EcommerceDDD.Core.Infrastructure.OpenTelemetry;

public static class ActivitySources
{
	/// <summary>Wolverine's own source: the outbox, broker and handler spans.</summary>
	public const string Wolverine = "Wolverine";
}
