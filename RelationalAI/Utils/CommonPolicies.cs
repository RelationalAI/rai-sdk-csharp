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
    using Polly.Retry;
    using Polly.Wrap;

    /// <summary>
    /// Defines a number of commonly used policies to allow retrying operations based on various conditions.
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
        public static RetryPolicy RequestErrorResilience { get; }

        /// <summary>
        /// Adds a policy that retries 5 times with exponential back-off on HTTP request sending errors,
        /// such as network connectivity issues, or 5xx status codes due to temporary server unavailability.
        /// </summary>
        /// <typeparam name="T">Type of result of an operation.</typeparam>
        /// <param name="policy">Initial policy instance.</param>
        /// <returns>Resulting policy instance.</returns>
        public static PolicyWrap<T> AddRequestErrorResilience<T>(this ISyncPolicy<T> policy)
        {
            return policy.Wrap(RequestErrorResilience);
        }

        /// <summary>
        /// Retries a specified number of times, using the 2^n function to calculate the duration to wait between retries
        /// based on the current retry attempt (exponential back-off).
        /// </summary>
        /// <typeparam name="T">Type of result of an operation.</typeparam>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="retryCount">The number of times to retry an operation.</param>
        /// <returns>Resulting policy instance.</returns>
        public static RetryPolicy<T> WaitExponentiallyAndRetry<T>(this PolicyBuilder<T> policyBuilder, int retryCount)
        {
            return policyBuilder.WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        private static RetryPolicy GetRequestErrorResiliencePolicy()
        {
            return Policy

                // The request failed due to an underlying issue such as network connectivity, DNS
                // failure, server certificate validation or timeout.
                .Handle<HttpRequestException>()

                // Response deserialization failed due to unsuccessful response received (5xx status code, etc.)
                // TODO: update after introducing exceptions hierarchy
                .Or<SystemException>()

                // Retry 5 times, using 2^n function to calculate the duration to wait between retries
                // based on the current retry attempt. In this case will wait for:
                //  2 ^ 1 = 2 seconds then
                //  2 ^ 2 = 4 seconds then
                //  2 ^ 3 = 8 seconds then
                //  2 ^ 4 = 16 seconds then
                //  2 ^ 5 = 32 seconds (1 min in total)
                // And rethrow the exception.
                .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}