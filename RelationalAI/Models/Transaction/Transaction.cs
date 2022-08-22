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

namespace RelationalAI.Models.Transaction
{
    public class Transaction : Entity
    {
        public string Region { get; set; }

        public string Database { get; set; }

        public string Engine { get; set; }

        public string Mode { get; set; }

        public string Source { get; set; }

        public bool Abort { get; set; }

        public bool ReadOnly { get; set; }

        public bool NoWaitDurable { get; set; }

        public int Version { get; set; }

        public Transaction(
            string region,
            string database,
            string engine,
            string mode,
            bool readOnly = false,
            string source = null)
        {
            Region = region;
            Database = database;
            Engine = engine;
            Mode = mode;
            ReadOnly = readOnly;
            Source = source;
        }

        /// <summary>
        /// Constructs the transaction payload.
        /// </summary>
        /// <param name="actions">List of actions.</param>
        /// <returns>The serialized transaction payload.</returns>
        public Dictionary<string, object> Payload(List<DbAction> actions)
        {
            var data = new Dictionary<string, object>
            {
                { "type", "Transaction" },
                { "mode", GetMode(Mode) },
                { "dbname", Database },
                { "abort", Abort },
                { "nowait_durable", NoWaitDurable },
                { "readonly", ReadOnly },
                { "version", Version },
                { "actions", DbAction.MakeActions(actions) }
            };
            if (Engine != null)
            {
                data.Add("computeName", Engine);
            }

            if (Source != null)
            {
                data.Add("source_dbname", Source);
            }

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
                { "region", Region },
                { "dbname", Database },
                { "compute_name", Engine },
                { "open_mode", GetMode(Mode) }
            };

            if (Source != null)
            {
                result.Add("source_dbname", Source);
            }

            return result;
        }

        private static string GetMode(string mode)
        {
            return mode ?? "OPEN";
        }
    }
}