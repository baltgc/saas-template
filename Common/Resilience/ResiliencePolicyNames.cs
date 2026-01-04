namespace saas_template.Common.Resilience;

public static class ResiliencePolicyNames
{
    public const string RetryPolicy = "RetryPolicy";
    public const string CircuitBreakerPolicy = "CircuitBreakerPolicy";
    public const string TimeoutPolicy = "TimeoutPolicy";
    public const string CombinedPolicy = "CombinedPolicy";
}

