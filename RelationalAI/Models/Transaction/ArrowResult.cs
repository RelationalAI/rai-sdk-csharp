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

namespace RelationalAI.Models.Transaction
{
    public class ArrowResult
    {
        public ArrowResult(string relationID, string filename, List<RecordBatch> records)
        {
            RelationID = relationID;
            Filename = filename;
            Records = records;
        }

        public string RelationID { get; set; }

        public string Filename { get; set; }

        public List<RecordBatch> Records { get; set; }
    }
}
