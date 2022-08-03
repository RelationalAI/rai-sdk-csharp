using System;
using System.Net.Http;
using Polly;
using Polly.Retry;

namespace RelationalAI.Utils
{
    public static class CommonPolicies
    {
        public static RetryPolicy RequestErrorResilience { get; }

        static CommonPolicies()
        {
            RequestErrorResilience = GetRequestErrorResiliencePolicy();
        }

        public static RetryPolicy GetRequestErrorResiliencePolicy()
        {
            return Policy
                // The request failed due to an underlying issue such as network connectivity, DNS
                // failure, server certificate validation or timeout.
                .Handle<HttpRequestException>()

                // 5** status code response received, etc. and response deserialization failed
                // TODO: update after introducing exceptions hierarchy
                .Or<SystemException>()

                // Exponential backoff pattern
                .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}