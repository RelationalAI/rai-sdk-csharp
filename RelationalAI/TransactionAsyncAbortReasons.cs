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
    public enum TransactionAsyncAbortReason
    {
        [EnumMember(Value = "system internal error")]
        SystemInternalError,
        [EnumMember(Value = "cancelled")]
        Cancelled,
        [EnumMember(Value = "engine shutting down")]
        EngineShuttingDown,
        [EnumMember(Value = "invalid request")]
        InvalidRequest,
        [EnumMember(Value = "too many requests")]
        TooManyRequests,
        [EnumMember(Value = "integrity constraint violation")]
        IntegrityConstraintViolation
    }

    public static class TransactionAsyncAbortReasons
    {
        public static string Value(this TransactionAsyncAbortReason reason)
        {
            return reason switch
            {
                TransactionAsyncAbortReason.SystemInternalError => "system internal error",
                TransactionAsyncAbortReason.Cancelled => "cancelled",
                TransactionAsyncAbortReason.EngineShuttingDown => "engine shutting down",
                TransactionAsyncAbortReason.InvalidRequest => "invalid request",
                TransactionAsyncAbortReason.TooManyRequests => "too many requests",
                TransactionAsyncAbortReason.IntegrityConstraintViolation => "integrity constraint violation",
                _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, "unknow async transaction abort reason")
            };
        }
    }
}
