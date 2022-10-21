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

namespace RelationalAI
{
    public enum TransactionAsyncAbortReason
    {
        InternalError,
        Cancelled,
        ShuttingDown,
        InvalidRequest,
        TooManyRequests,
        IntegrityConstraintViolation
    }

    public static class TransactionAsyncAbortReasons
    {
        public static string Value(this TransactionAsyncAbortReason reason)
        {
            return reason switch
            {
                TransactionAsyncAbortReason.InternalError => "system internal error",
                TransactionAsyncAbortReason.Cancelled => "cancelled",
                TransactionAsyncAbortReason.ShuttingDown => "engine shutting down",
                TransactionAsyncAbortReason.InvalidRequest => "invalid request",
                TransactionAsyncAbortReason.TooManyRequests => "too many requests",
                TransactionAsyncAbortReason.IntegrityConstraintViolation => "integrity constraint violation",
                _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, "unknow async transaction abort reason")
            };
        }
    }
}
