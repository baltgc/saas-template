using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Net;

namespace saas_template.Extensions;

public static class ResilienceExtensions
{
    public static IServiceCollection AddResiliencePolicies(this IServiceCollection services)
    {
        // Retry policy with exponential backoff
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    // Log retry attempts if needed
                });

        // Circuit breaker policy - opens after 5 consecutive failures, stays open for 30 seconds
        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, duration) =>
                {
                    // Circuit breaker opened
                },
                onReset: () =>
                {
                    // Circuit breaker reset
                });

        // Timeout policy - 10 second timeout
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);

        // Combine policies: Retry -> Circuit Breaker -> Timeout
        var resiliencePolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);

        // Register policies for HTTP clients
        services.AddHttpClient()
            .AddPolicyHandler(resiliencePolicy);

        return services;
    }
}

