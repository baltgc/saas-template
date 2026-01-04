namespace saas_template.Common.Helpers;

public static class CorrelationIdHelper
{
    public const string CorrelationIdHeaderName = "X-Correlation-ID";
    public const string CorrelationIdItemKey = "CorrelationId";

    public static string GetCorrelationId(HttpContext context)
    {
        if (context.Items.TryGetValue(CorrelationIdItemKey, out var correlationId) && correlationId is string id)
        {
            return id;
        }

        return context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault() ?? Guid.NewGuid().ToString();
    }
}

