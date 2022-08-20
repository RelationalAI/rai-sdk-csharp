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

using System.Collections.Generic;
using System.Linq;

namespace RelationalAI
{
    public class TransactionAsync : Entity
    {
        public string Database { get; set; }

        public string Engine { get; set; }

        public string Source { get; set; }

        public bool ReadOnly { get; set; }

        public Dictionary<string, string> Inputs { get; set; }

        public TransactionAsync(
            string database,
            string engine,
            bool readOnly = false,
            string source = null,
            Dictionary<string, string> inputs = null)
        {
            Database = database;
            Engine = engine;
            ReadOnly = readOnly;
            Source = source;
            Inputs = inputs ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Constructs the transaction payload.
        /// </summary>
        /// <returns>The serialized transaction payload.</returns>
        public Dictionary<string, object> Payload()
        {
            var data = new Dictionary<string, object>
            {
                { "dbname", Database },
                { "readonly", ReadOnly },
                { "engine_name", Engine },
                { "query", Source }
            };

            var actionInputs = Inputs.Select(entry => DbAction.MakeQueryActionInput(entry.Key, entry.Value)).ToList();

            data.Add("v1_inputs", actionInputs);

            return data;
        }

        /// <summary>
        /// Creates the query params corresponding to the transaction state.
        /// </summary>
        /// <returns>The query params.</returns>
        public Dictionary<string, string> QueryParams()
        {
            var result = new Dictionary<string, string>
            {
                { "dbname", Database },
                { "engine_name", Engine }
            };

            return result;
        }
    }
}
