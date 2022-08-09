/*
 * Copyright 2022 RelationalAI, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace RelationalAI.Utils
{
    using System;
    using System.Net.Http;
    using Polly;

    /// <summary>
    /// Defines a number of commonly used policies and extension methods to use them
    /// to allow retrying operations based on various conditions.
    /// </summary>
    public static class CommonPolicies
    {
        static CommonPolicies()
        {
            RequestErrorResilience = GetRequestErrorResiliencePolicy();
        }

        /// <summary>
        /// Gets a policy that retries 5 times with exponential back-off on HTTP request sending errors,
        /// such as network connectivity issues, or 5xx status codes due to temporary server unavailability.
        /// </summary>
        public static Policy RequestErrorResilience { get; }

        /// <summary>
        /// Creates a policy that can be used to produce synchronous API for async calls that may be taking a long
        /// time to complete. Does not time out, therefore use with caution.
        /// Uses exponential back-off starting with a 2 seconds delay up to <paramref name="delayThreshold"/> seconds.
        /// Retries on HTTP request sending errors.
        /// </summary>
        /// <typeparam name="T">Result type of an operation.</typeparam>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="delayThreshold">Max delay between retry attempts in seconds. Defaults to 30 seconds.</param>
        /// <returns>Resulting policy instance.</returns>
        public static Policy<T> RetryForeverWithBoundedDelay<T>(this PolicyBuilder<T> policyBuilder, int delayThreshold = 30)
        {
            return policyBuilder
                .WaitAndRetryForever(retryAttempt => GetBoundedRetryDelay(retryAttempt, delayThreshold))
                .AddRequestErrorResilience();
        }

        /// <summary>
        /// Creates a policy that can be used to produce synchronous API for async calls which may be taking less than 5 min
        /// to complete, otherwise throws TimeoutRejectedException.
        /// Uses exponential back-off starting with a 2 seconds delay up to 15 seconds and then retrying every 15 seconds.
        /// Retries on HTTP request sending errors.
        /// </summary>
        /// <typeparam name="T">Result type of an operation.</typeparam>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <returns>Resulting policy instance.</returns>
        public static Policy<T> Retry5Min<T>(this PolicyBuilder<T> policyBuilder)
        {
            return policyBuilder.AddBoundedRetryPolicy(15, 5 * 60);
        }

        /// <summary>
        /// Creates a policy that can be used to produce synchronous API for async calls which may be taking less than 30 min
        /// to complete, otherwise throws TimeoutRejectedException.
        /// Uses exponential back-off starting with a 2 seconds delay up to 15 seconds and then retrying every 15 seconds.
        /// Retries on HTTP request sending errors.
        /// </summary>
        /// <typeparam name="T">Result type of an operation.</typeparam>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <returns>Resulting policy instance.</returns>
        public static Policy<T> Retry30Min<T>(this PolicyBuilder<T> policyBuilder)
        {
            return policyBuilder.AddBoundedRetryPolicy(15, 30 * 60);
        }

        /// <summary>
        /// Adds a policy that retries 5 times with exponential back-off on HTTP request sending errors,
        /// such as network connectivity issues, or 5xx status codes due to temporary server unavailability.
        /// </summary>
        /// <typeparam name="T">Type of result of an operation.</typeparam>
        /// <param name="policy">Initial policy instance.</param>
        /// <returns>Resulting policy instance.</returns>
        public static Policy<T> AddRequestErrorResilience<T>(this ISyncPolicy<T> policy)
        {
            return policy.Wrap(RequestErrorResilience);
        }

        private static Policy<T> AddBoundedRetryPolicy<T>(this PolicyBuilder<T> policyBuilder, int delayThreshold, int timeoutSeconds)
        {
            var timeoutPolicy = Policy.Timeout(TimeSpan.FromSeconds(timeoutSeconds));
            var retryPolicy = policyBuilder.RetryForeverWithBoundedDelay(delayThreshold);
            return timeoutPolicy.Wrap(retryPolicy);
        }

        private static Policy GetRequestErrorResiliencePolicy()
        {
            return Policy

                // The request failed due to an underlying issue such as network connectivity, DNS
                // failure, server certificate validation or timeout.
                .Handle<HttpRequestException>()

                // Response deserialization failed due to unsuccessful response received (5xx status code, etc.)
                // TODO: update after introducing exceptions hierarchy
                .Or<SystemException>()

                // Retry 5 times. In this case will wait for: 2 + 4 + 8 + 16 + 30 seconds
                // And rethrow the exception.
                .WaitAndRetry(5, retryAttempt => GetBoundedRetryDelay(retryAttempt, 30));
        }

        private static TimeSpan GetBoundedRetryDelay(int retryAttempt, int delayThreshold)
        {
            var exponentialDelay = Math.Pow(2, retryAttempt); // expected delay for the Nth retry attempt
            var retryDelay = Math.Min(exponentialDelay, delayThreshold);
            return TimeSpan.FromSeconds(retryDelay);
        }
    }
}