// <copyright file="TransactionAsyncResult.cs" company="PlaceholderCompany">
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
    using Relationalai.Protocol;

    public class TransactionAsyncResult : Entity
    {
        public TransactionAsyncResult(
            TransactionAsyncCompactResponse transaction,
            List<ArrowRelation> results,
            MetadataInfo metadata,
            List<object> problems,
            bool gotCompleteResult = false)
        {
            this.GotCompleteResult = gotCompleteResult;
            this.Transaction = transaction;
            this.Results = results;
            this.Metadata = metadata;
            this.Problems = problems;
        }

        public bool GotCompleteResult { get; set; }

        public TransactionAsyncCompactResponse Transaction { get; set; }

        public List<ArrowRelation> Results { get; set; }

        public MetadataInfo Metadata { get; set; }

        public List<object> Problems { get; set; }
    }
}
