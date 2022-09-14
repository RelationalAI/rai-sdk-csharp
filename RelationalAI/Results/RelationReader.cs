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

using System.Collections;
using System.Collections.Generic;
using Apache.Arrow;
using ConsoleTables;
using RelationalAI.Models.Transaction;
using Relationalai.Protocol;
using RelationalAI.Results.Models;

namespace RelationalAI.Results
{
    public class RelationReader
    {
        public RelationReader(ArrowRelation relation = null)
        {
            if (relation != null)
            {
                Table = relation.Table;
                RelationID = relation.Metadata;
                ColumnDefs = GetColumnDefsFromProtobuf();
            }
        }

        public Table Table { get; set; }

        public RelationId RelationID { get; set; }

        public List<ColumnDef> ColumnDefs { get; set; }

        public List<object> Tuples()
        {
            var output = new List<object>();

            if (Utils.IsFullySpecialized(ColumnDefs))
            {
                var spec = new List<object>();
                foreach (var colDef in ColumnDefs)
                {
                    spec.Add(Utils.ConvertValue(colDef.TypeDef, null));
                }

                output.Add(spec);
            }

            var rows = RowsRawValues();
            foreach (var row in rows)
            {
                output.Add(Utils.ArrowRowToValues(row as List<object>, ColumnDefs));
            }

            return output;
        }

        public List<object> Tuple(int index)
        {
            var output = new List<object>();

            if (Utils.IsFullySpecialized(ColumnDefs))
            {
                var spec = new List<object>();
                foreach (var colDef in ColumnDefs)
                {
                    spec.Add(Utils.ConvertValue(colDef.TypeDef, null));
                }

                output.AddRange(spec);
            }

            var rows = RowsRawValues();
            if (rows.Count > index)
            {
                output.AddRange((IEnumerable<object>)Utils.ArrowRowToValues(rows[index] as List<object>, ColumnDefs));
            }

            return output;
        }

        public int ColumnCount()
        {
            return Table.ColumnCount;
        }

        public long TuplesCount()
        {
            return Table.RowCount;
        }

        public RelationReader Physical()
        {
            var reader = new RelationReader
            {
                ColumnDefs = new List<ColumnDef>(),
                Table = Table
            };

            foreach (var colDef in ColumnDefs)
            {
                if (colDef.TypeDef.Type != "Constant")
                {
                    reader.ColumnDefs.Add(colDef);
                }
            }

            return reader;
        }

        public void Print()
        {
            var headers = new List<string>();
            foreach (var colDef in ColumnDefs)
            {
                if (colDef.TypeDef.Type == "Constant")
                {
                    var value = colDef.TypeDef.Value as TypeDef;
                    headers.Add(value.Type);
                }
                else
                {
                    headers.Add(colDef.TypeDef.Type);
                }
            }

            var table = new ConsoleTable(headers.ToArray());

            foreach (var tuple in Tuples())
            {
                table.AddRow((tuple as List<object>).ToArray());
            }

            table.Write();
        }

        public List<TypeDef> TypeDefs()
        {
            var output = new List<TypeDef>();

            foreach (var colDef in ColumnDefs)
            {
                output.Add(colDef.TypeDef);
            }

            return output;
        }

        public List<ColumnDef> GetColumnDefsFromProtobuf()
        {
            List<ColumnDef> colDefs = new List<ColumnDef>();
            var arrowIndex = 0;

            foreach (var relType in RelationID.Arguments)
            {
                var typeDef = Utils.GetColumnDefFromProtobuf(relType);
                var colDef = new ColumnDef(typeDef, relType, arrowIndex);
                if (typeDef.Type != "Constant")
                {
                    arrowIndex++;
                }

                colDefs.Add(colDef);
            }

            return colDefs;
        }

        private Dictionary<string, IList> ColumnsToDict()
        {
            var output = new Dictionary<string, IList>();
            for (int i = 0; i < Table.ColumnCount; i++)
            {
                var column = Table.Column(i);
                var values = ColumnRawValues(column);

                var key = Table.Column(i).Name;
                if (output.ContainsKey(key))
                {
                    output[key].Add(values);
                }
                else
                {
                    output.Add(key, values);
                }
            }

            return output;
        }

        private IList ColumnRawValues(Column column)
        {
            var output = new List<object>();

            for (int j = 0; j < column.Data.ArrayCount; j++)
            {
                var values = Utils.ArrowArrayToArray(column.Data.Array(j));
                foreach (var v in values)
                {
                    output.Add(v);
                }
            }

            return output;
        }

        private IList RowsRawValues()
        {
            var output = new List<object>();

            var dict = ColumnsToDict();
            for (int rowIndex = 0; rowIndex < Table.RowCount; rowIndex++)
            {
                var arr = new List<object>();
                foreach (var key in dict.Keys)
                {
                    arr.Add(dict[key][rowIndex]);
                }

                output.Add(arr);
            }

            return output;
        }
    }
}