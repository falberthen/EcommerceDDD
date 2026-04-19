namespace EcommerceDDD.Core.Infrastructure.OpenTelemetry;

public interface ITraceable
{
    IEnumerable<KeyValuePair<string, string>> GetSpanTags();
}
