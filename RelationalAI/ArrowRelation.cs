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
    public class ArrowRelation : Entity
    {
        public string RelationId { get; set; }
        public List<object> Table { get; set; }

        public ArrowRelation(string relationId, List<object> table)
        {
            RelationId = relationId;
            Table = table;
        }

        public override bool Equals(object obj)
        {
            if (obj is ArrowRelation)
            {
                var that = obj as ArrowRelation;

                return RelationId == that.RelationId && Table.SequenceEqual(that.Table);
            }

            return false;
        }

        public override int GetHashCode()
        {
            var hashcode = RelationId.GetHashCode();
            hashcode ^= Table.GetHashCode();
            return hashcode;
        }
    }
}
