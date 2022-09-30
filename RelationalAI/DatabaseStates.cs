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
    /// Represents all database lifetime states.
    /// </summary>
    public enum DatabaseState
    {
        Created,
        Creating,
        CreationFailed,
        Deleted
    }

    /// <summary>
    /// Extension methods for DatabaseState enum such as conversion to string, final states identification.
    /// </summary>
    public static class DatabaseStates
    {
        /// <summary>
        /// Converts <paramref name="state"/> to string value RAI REST API returns.
        /// </summary>
        /// <param name="state">Database state.</param>
        /// <returns>The string value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Gets thrown when unsupported enum value is passed.</exception>
        public static string Value(this DatabaseState state)
        {
            return state switch
            {
                DatabaseState.Created => "CREATED",
                DatabaseState.Creating => "CREATING",
                DatabaseState.CreationFailed => "CREATION_FAILED",
                DatabaseState.Deleted => "DELETED",
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Database state is not supported")
            };
        }

        /// <summary>
        /// Identifies if <paramref name="state"/> is the final state of database,
        /// i.e. database can't transition to a different state from it.
        /// </summary>
        /// <param name="state">The database state to check.</param>
        /// <returns>If the state if final.</returns>
        public static bool IsFinalState(this DatabaseState state)
        {
            return state == DatabaseState.CreationFailed ||
                   state == DatabaseState.Deleted;
        }
    }
}