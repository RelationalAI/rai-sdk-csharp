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

namespace RelationalAI.Model.Transaction
{
    public class TransactionAsyncResult : Entity
    {
        public bool GotCompleteResult { get; set; }

        public TransactionAsyncCompactResponse Transaction { get; set; }

        public List<ArrowRelation> Results { get; set; }

        public List<TransactionAsyncMetadataResponse> Metadata { get; set; }

        public List<object> Problems { get; set; }

        public TransactionAsyncResult(
            TransactionAsyncCompactResponse transaction,
            List<ArrowRelation> results,
            List<TransactionAsyncMetadataResponse> metadata,
            List<object> problems,
            bool gotCompleteResult = false)
        {
            GotCompleteResult = gotCompleteResult;
            Transaction = transaction;
            Results = results;
            Metadata = metadata;
            Problems = problems;
        }
    }
}
