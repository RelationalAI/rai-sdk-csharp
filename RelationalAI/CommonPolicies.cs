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

using System;
using System.Net.Http;
using Polly;

namespace RelationalAI
{
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

        private static AsyncPolicy RequestErrorResilience { get; }

        /// <summary>
        /// Creates a policy that can be used to produce synchronous API for async calls that may be taking a long
        /// time to complete. Does not time out, therefore use with caution.
        /// Uses exponential back-off starting with a 2 seconds delay up to <paramref name="delayThreshold"/> seconds.
        /// Retries on HTTP request sending errors.
        /// </summary>
        /// <typeparam name="T">Result type of an operation.</typeparam>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="startTime">The transaction start time</param>
        /// <param name="overheadRate">the overhead % to add through polling</param>
        /// <param name="delayThreshold">Max delay between retry attempts in seconds. Defaults to 30 seconds.</param>
        /// <returns>Resulting policy instance.</returns>
        public static AsyncPolicy<T> RetryForeverWithBoundedDelay<T>(this PolicyBuilder<T> policyBuilder, DateTime? startTime = null, double overheadRate = 0.1, int delayThreshold = 30)
        {
            var time = startTime == null ? DateTime.UtcNow : (DateTime)startTime;
            return policyBuilder
                .WaitAndRetryForeverAsync(retryAttempt => GetBoundedRetryDelay(time, overheadRate, delayThreshold))
                .AddRequestErrorResilience();
        }

        /// <summary>
        /// Creates a policy that can be used to produce synchronous API for async calls which may be taking less than 15 min
        /// to complete, otherwise throws TimeoutRejectedException.
        /// Uses exponential back-off with an overhead % of the time the transaction has been running so far up to 15 seconds and then retrying every 15 seconds.
        /// Retries on HTTP request sending errors.
        /// </summary>
        /// <typeparam name="T">Result type of an operation.</typeparam>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="startTime">The transaction start time</param>
        /// <param name="overheadRate">the overhead % to add through polling</param>
        /// <returns>Resulting policy instance.</returns>
        public static AsyncPolicy<T> Retry15Min<T>(this PolicyBuilder<T> policyBuilder, DateTime? startTime = null, double overheadRate = 0.1)
        {
            var time = startTime == null ? DateTime.UtcNow : (DateTime)startTime;
            return policyBuilder.AddBoundedRetryPolicy(time, overheadRate, 15, 15 * 60);
        }

        /// <summary>
        /// Creates a policy that can be used to produce synchronous API for async calls which may be taking less than 30 min
        /// to complete, otherwise throws TimeoutRejectedException.
        /// Uses exponential back-off with an overhead % of the time the transaction has been running so far up to 15 seconds and then retrying every 15 seconds.
        /// Retries on HTTP request sending errors.
        /// </summary>
        /// <typeparam name="T">Result type of an operation.</typeparam>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="startTime">The transaction start time</param>
        /// <param name="overheadRate">the overhead % to add through polling</param>
        /// <returns>Resulting policy instance.</returns>
        public static AsyncPolicy<T> Retry30Min<T>(this PolicyBuilder<T> policyBuilder, DateTime? startTime = null, double overheadRate = 0.1)
        {
            var time = startTime == null ? DateTime.UtcNow : (DateTime)startTime;
            return policyBuilder.AddBoundedRetryPolicy(time, overheadRate, 15, 30 * 60);
        }

        /// <summary>
        /// Adds a policy that retries 5 times with exponential back-off on HTTP request sending errors,
        /// such as network connectivity issues, or 5xx status codes due to temporary server unavailability.
        /// </summary>
        /// <typeparam name="T">Type of result of an operation.</typeparam>
        /// <param name="policy">Initial policy instance.</param>
        /// <returns>Resulting policy instance.</returns>
        private static AsyncPolicy<T> AddRequestErrorResilience<T>(this IAsyncPolicy<T> policy)
        {
            return policy.WrapAsync(RequestErrorResilience);
        }

        private static AsyncPolicy<T> AddBoundedRetryPolicy<T>(this PolicyBuilder<T> policyBuilder, DateTime startTime, double overheadRate, int delayThreshold, int timeoutSeconds)
        {
            var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromSeconds(timeoutSeconds));
            var retryPolicy = policyBuilder.RetryForeverWithBoundedDelay(startTime, overheadRate, delayThreshold);
            return timeoutPolicy.WrapAsync(retryPolicy);
        }

        private static AsyncPolicy GetRequestErrorResiliencePolicy()
        {
            return Policy

                // The request failed due to an underlying issue such as network connectivity, DNS
                // failure, server certificate validation or timeout.
                .Handle<HttpRequestException>()

                // Server error response received (5xx status code, etc.)
                .Or<ApiException>()

                // Retry 5 times with 10% overhead of the time the transaction has been running so far
                // And rethrow the exception.
                .WaitAndRetryAsync(5, retryAttempt => GetBoundedRetryDelay(DateTime.UtcNow, 0.10, 30));
        }

        // Adds a % overhead of the time the transaction has been running so far
        private static TimeSpan GetBoundedRetryDelay(DateTime startTime, double overheadRate, int maxDelayThreshold)
        {
            var currentDelay = DateTime.UtcNow - startTime;
            var duration = currentDelay.TotalMilliseconds * overheadRate;
            return TimeSpan.FromMilliseconds(Math.Min(duration, maxDelayThreshold * 1000));
        }
    }
}