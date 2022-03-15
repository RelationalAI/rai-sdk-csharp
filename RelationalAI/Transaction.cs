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
    using Newtonsoft.Json;

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
            this.Region = region;
            this.Database = database;
            this.Engine = engine;
            this.Mode = mode;
            this.ReadOnly = readOnly;
            this.Source = source;
        }

        // Construct the transaction payload and return serialized JSON string.
        public Dictionary<string, object> Payload(List<DbAction> actions)
        {
            var data = new Dictionary<string, object>();
            data.Add("type", "Transaction");
            data.Add("mode", Transaction.GetMode(this.Mode));
            data.Add("dbname", this.Database);
            data.Add("abort", this.Abort);
            data.Add("nowait_durable", this.NoWaitDurable);
            data.Add("readonly", this.ReadOnly);
            data.Add("version", this.Version);
            data.Add("actions", DbAction.MakeActions(actions));
            if (this.Engine != null)
            {
                data.Add("computeName", this.Engine);
            }
            if (this.Source != null)
            {
                data.Add("source_dbname", this.Source);
            }

            return data;
        }

        // Returns the query params corresponding to the transaction state.
        public Dictionary<string, string> QueryParams()
        {
            Dictionary<string, string> result = new Dictionary<string, string>()
            { 
                { "region", this.Region },
                { "dbname", this.Database },
                { "compute_name", this.Engine },
                { "open_mode", Transaction.GetMode(this.Mode) },
            };

            if (this.Source != null)
            {
                result.Add("source_dbname", this.Source);
            }

            return result;
        }

        private static string GetMode(string mode)
        {
            if (mode == null)
            {
                return "OPEN";
            }

            return mode;
        }
    }
}