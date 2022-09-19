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

namespace RelationalAI.Models.Engine
{
    /// <summary>
    /// Methods for EngineState determinations such as terminal states identification.
    /// </summary>
    public static class EngineStates
    {
        public static readonly string Deleted = "DELETED";
        public static readonly string DeletionFailed = "DELETION_FAILED";
        public static readonly string Provisioned = "PROVISIONED";
        public static readonly string ProvisionFailed = "PROVISION_FAILED";
        public static readonly string RequestFailed = "REQUEST_FAILED";

        /// <summary>
        /// Identifies if <paramref name="state"/> is the final state of engine,
        /// i.e. engine can't transition to a different state from it.
        /// </summary>
        /// <param name="state">The engine state to check.</param>
        /// <returns>If the state if final.</returns>
        public static bool IsFinalState(string state)
        {
            return state == ProvisionFailed ||
                   state == Deleted ||
                   state == DeletionFailed ||
                   state == RequestFailed;
        }

        /// <summary>
        /// Identifies if <paramref name="currentState"/> is terminal:
        /// either final state, or matching <paramref name="targetState"/>.
        /// </summary>
        /// <param name="currentState">The current state.</param>
        /// <param name="targetState">The target state.</param>
        /// <returns>If the current state is terminal.</returns>
        public static bool IsTerminalState(string currentState, string targetState)
        {
            return currentState == targetState || IsFinalState(currentState);
        }
    }
}