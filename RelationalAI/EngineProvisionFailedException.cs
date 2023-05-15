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
    /// Represents error thrown when engine requested to provision failed to get provisioned.
    /// </summary>
    public class EngineProvisionFailedException : Exception
    {
        public EngineProvisionFailedException(Engine engine)
            : base($"Engine with name `{engine.Name}` failed to provision")
        {
            Engine = engine;
        }

        public EngineProvisionFailedException(string engine)
            : base($"Engine with name {engine} failed to provision")
        { }

        /// <summary>
        /// Gets the name of the engine that failed to provision.
        /// </summary>
        public Engine Engine { get; }
    }
}
