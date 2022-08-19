// <copyright file="TransactionAsync.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

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
namespace RelationalAI
{
    using System.Collections.Generic;

    public class TransactionAsync : Entity
    {
        public TransactionAsync(
            string database,
            string engine,
            bool readOnly = false,
            string source = null,
            Dictionary<string, string> inputs = null)
        {
            this.Database = database;
            this.Engine = engine;
            this.ReadOnly = readOnly;
            this.Source = source;
            this.Inputs = inputs ?? new Dictionary<string, string>();
        }

        public string Database { get; set; }

        public string Engine { get; set; }

        public string Source { get; set; }

        public bool ReadOnly { get; set; }

        public Dictionary<string, string> Inputs { get; set; }

        // Construct the transaction payload and return serialized JSON string.
        public Dictionary<string, object> Payload()
        {
            var data = new Dictionary<string, object>
            {
                { "dbname", this.Database },
                { "readonly", this.ReadOnly },
                { "engine_name", this.Engine },
                { "query", this.Source },
            };

            var actionInputs = new List<DbAction>();
            foreach (var entry in this.Inputs)
            {
                var actionInput = DbAction.MakeQueryActionInput(entry.Key, entry.Value);
                actionInputs.Add(actionInput);
            }

            data.Add("v1_inputs", actionInputs);

            return data;
        }

        // Returns the query params corresponding to the transaction state.
        public Dictionary<string, string> QueryParams()
        {
            Dictionary<string, string> result = new Dictionary<string, string>()
            {
                { "dbname", this.Database },
                { "engine_name", this.Engine },
            };

            return result;
        }
    }
}
