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
using System.Runtime.Serialization;

namespace RelationalAI
{
    public static class TransactionAsyncAbortReason
    {
        public const string SystemInternalError = "system internal error";
        public const string Cancelled = "cancelled";
        public const string EngineShuttingDown = "engine shutting down";
        public const string InvalidRequest = "invalid request";
        public const string TooManyReqeuests = "too many requests";
        public const string IntegrityConstraintViolation = "integrity constraint violation";
    }
}
