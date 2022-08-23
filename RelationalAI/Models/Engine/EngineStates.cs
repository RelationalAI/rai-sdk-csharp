﻿/*
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
    /// Represents all engine lifetime states.
    /// </summary>
    public enum EngineState
    {
        Requested,
        Provisioning,
        Registering,
        Provisioned,
        ProvisionFailed,
        DeleteRequested,
        Stopping,
        Deleting,
        Deleted,
        DeletionFailed
    }

    /// <summary>
    /// Extension methods for EngineState enum such as conversion to string, terminal states identification.
    /// </summary>
    public static class EngineStates
    {
        /// <summary>
        /// Converts <paramref name="state"/> to string value RAI REST API returns.
        /// </summary>
        /// <param name="state">Engine state.</param>
        /// <returns>The string value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Gets thrown when unsupported enum value is passed.</exception>
        public static string Value(this EngineState state)
        {
            return state switch
            {
                EngineState.Requested => "REQUESTED",
                EngineState.Provisioning => "PROVISIONING",
                EngineState.Registering => "REGISTERING",
                EngineState.Provisioned => "PROVISIONED",
                EngineState.ProvisionFailed => "PROVISION_FAILED",
                EngineState.DeleteRequested => "DELETE_REQUESTED",
                EngineState.Stopping => "STOPPING",
                EngineState.Deleting => "DELETING",
                EngineState.Deleted => "DELETED",
                EngineState.DeletionFailed => "DELETION_FAILED",
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Engine state is not supported")
            };
        }

        /// <summary>
        /// Identifies if <paramref name="state"/> is the final state of engine,
        /// i.e. engine can't transition to a different state from it.
        /// </summary>
        /// <param name="state">The engine state to check.</param>
        /// <returns>If the state if final.</returns>
        public static bool IsFinalState(this EngineState state)
        {
            return state == EngineState.ProvisionFailed ||
                   state == EngineState.Deleted ||
                   state == EngineState.DeletionFailed;
        }

        /// <summary>
        /// Identifies if <paramref name="currentState"/> is terminal:
        /// either final state, or matching <paramref name="targetState"/>.
        /// </summary>
        /// <param name="currentState">The current state.</param>
        /// <param name="targetState">The target state.</param>
        /// <returns>If the current state is terminal.</returns>
        public static bool IsTerminalState(this EngineState currentState, EngineState targetState)
        {
            return currentState == targetState || currentState.IsFinalState();
        }
    }
}