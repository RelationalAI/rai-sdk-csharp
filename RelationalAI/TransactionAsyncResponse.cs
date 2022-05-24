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
    using Newtonsoft.Json;

    public class TransactionAsyncResponse : TransactionAsyncCompactResponse
    {

        [JsonProperty("account_name", Required = Required.Always)]
        public string AccountName { get; set; }

        [JsonProperty("created_by", Required = Required.Always)]
        public string CreatedBy { get; set; }

        [JsonProperty("created_on", Required = Required.Always)]
        public long CreatedOn { get; set; }

        [JsonProperty("finished_at", Required = Required.Default)]
        public long FinishedAt { get; set; }

        [JsonProperty("database_name", Required = Required.Always)]
        public string DatabaseName { get; set; }

        [JsonProperty("read_only", Required = Required.Always)]
        public bool ReadOnly { get; set; }

        [JsonProperty("query", Required = Required.Always)]
        public string Query { get; set; }

        [JsonProperty("last_requested_interval", Required = Required.Always)]
        public string LastRequestedInterval { get; set; }

        public TransactionAsyncResponse(
            string id,
            string state,
            string accountName,
            string createdBy,
            long createdOn,
            long finishedAt,
            string databaseName,
            bool readOnly,
            string query,
            string lastRequestedInterval
        ) : base(id, state)
        {
            this.AccountName = accountName;
            this.CreatedBy = createdBy;
            this.CreatedOn = createdOn;
            this.FinishedAt = finishedAt;
            this.DatabaseName = databaseName;
            this.ReadOnly = readOnly;
            this.Query = query;
            this.LastRequestedInterval = lastRequestedInterval;
        }
    }
}
