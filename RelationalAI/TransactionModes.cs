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
    /// <summary>
    /// Represents supported transaction execution modes.
    /// </summary>
    public enum TransactionMode
    {
        Open,
        Create,
        CreateOverwrite,
        OpenOrCreate,
        Clone,
        CloneOverwrite
    }

    /// <summary>
    /// Extension methods for TransactionMode enum such as conversion to string value RAI REST API expects.
    /// </summary>
    public static class TransactionModes
    {
        /// <summary>
        /// Converts <paramref name="mode"/> to string value RAI REST API expects.
        /// </summary>
        /// <param name="mode">The transaction mode.</param>
        /// <returns>The string value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Gets thrown when unsupported enum value is passed.</exception>
        public static string Value(this TransactionMode mode)
        {
            return mode switch
            {
                TransactionMode.Open => "OPEN",
                TransactionMode.Create => "CREATE",
                TransactionMode.CreateOverwrite => "CREATE_OVERWRITE",
                TransactionMode.OpenOrCreate => "OPEN_OR_CREATE",
                TransactionMode.Clone => "CLONE",
                TransactionMode.CloneOverwrite => "CLONE_OVERWRITE",
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Transaction mode is not supported")
            };
        }
    }
}