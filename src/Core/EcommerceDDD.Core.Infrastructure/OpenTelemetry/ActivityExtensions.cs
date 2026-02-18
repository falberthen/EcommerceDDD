using System.Diagnostics;

namespace EcommerceDDD.Core.Infrastructure.OpenTelemetry;

public static class ActivityExtensions
{
    /// <summary>
    /// Gets the current TraceId for correlation across services
    /// </summary>
    public static string? GetTraceId()
    {
        return Activity.Current?.TraceId.ToString();
    }

    /// <summary>
    /// Gets the current SpanId
    /// </summary>
    public static string? GetSpanId()
    {
        return Activity.Current?.SpanId.ToString();
    }

    /// <summary>
    /// Adds baggage item that will be propagated across service boundaries
    /// Example: AddBaggage("order.id", orderId)
    /// </summary>
    public static void AddBaggage(string key, string value)
    {
        Activity.Current?.AddBaggage(key, value);
    }

    /// <summary>
    /// Gets baggage value that was propagated from another service
    /// </summary>
    public static string? GetBaggage(string key)
    {
        return Activity.Current?.GetBaggageItem(key);
    }

    /// <summary>
    /// Adds a tag to the current activity for better trace filtering
    /// Example: AddTag("order.id", orderId)
    /// </summary>
    public static void AddTag(string key, string? value)
    {
        Activity.Current?.SetTag(key, value);
    }

    /// <summary>
    /// Adds an event to the current activity timeline
    /// Example: AddEvent("OrderPlaced", new { OrderId = orderId, Amount = total })
    /// </summary>
    public static void AddEvent(string name, object? data = null)
    {
        if (Activity.Current == null) return;

        var activityEvent = new ActivityEvent(name);

        if (data != null)
        {
            foreach (var prop in data.GetType().GetProperties())
            {
                var value = prop.GetValue(data);
                activityEvent = activityEvent.AddTag(prop.Name, value);
            }
        }

        Activity.Current.AddEvent(activityEvent);
    }
}
