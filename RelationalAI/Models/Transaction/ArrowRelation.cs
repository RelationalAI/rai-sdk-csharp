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
using Apache.Arrow;
using Relationalai.Protocol;

namespace RelationalAI.Models.Transaction
{
    public class ArrowRelation : Entity
    {
        public ArrowRelation(string relationId, List<RecordBatch> records, RelationId metadata)
        {
            RelationId = relationId;
            Records = records;
            Metadata = metadata;
        }

        public string RelationId { get; }

        public List<RecordBatch> Records { get; }

        public RelationId Metadata { get; }

        public override bool Equals(object obj)
        {
            if (obj is ArrowRelation arrowRelation)
            {
                return RelationId == arrowRelation.RelationId &&
                    Records == arrowRelation.Records &&
                    Metadata == arrowRelation.Metadata;
            }

            return false;
        }

        public override int GetHashCode()
        {
            var hashcode = RelationId.GetHashCode();
            hashcode ^= Records.GetHashCode();
            return hashcode;
        }
    }
}
