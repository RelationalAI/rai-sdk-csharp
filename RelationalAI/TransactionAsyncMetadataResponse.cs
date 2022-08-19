// <copyright file="TransactionAsyncMetadataResponse.cs" company="PlaceholderCompany">
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
    using System.Linq;
    using Newtonsoft.Json;

    public class TransactionAsyncMetadataResponse : Entity
    {
        public TransactionAsyncMetadataResponse(string relationId, List<string> types)
        {
            this.RelationId = relationId;
            this.Types = types;
        }

        [JsonProperty("relationId", Required = Required.Always)]
        public string RelationId { get; set; }

        [JsonProperty("types", Required = Required.Always)]
        public List<string> Types { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TransactionAsyncMetadataResponse)
            {
                var that = obj as TransactionAsyncMetadataResponse;

                return this.RelationId == that.RelationId && Enumerable.SequenceEqual(this.Types, that.Types);
            }

            return false;
        }

        public override int GetHashCode()
        {
            var hashcode = this.RelationId.GetHashCode();
            hashcode ^= this.Types.GetHashCode();
            return hashcode;
        }
    }
}
