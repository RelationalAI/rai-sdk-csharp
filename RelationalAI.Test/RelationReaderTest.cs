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
using Apache.Arrow.Types;
using Google.Protobuf;
using Newtonsoft.Json;
using Relationalai.Protocol;
using RelationalAI.Results;
using RelationalAI.Results.Models;
using Xunit;

namespace RelationalAI.Test
{
    public class RelationReaderTest : UnitTest
    {
        // query
        // def output =
        // (#1, :foo, "w", :bar, 'a', 1);
        // (#1, :foo, "x", :bar, 'b', 2);
        // (#1, :foo, "y", :bar, 'c', 3);
        // (#1, :foo, "z", :bar, 'd', 4)

        [Fact]
        public void TypesDefinitionTests()
        {
            var expected = new List<ColumnDef>
            {
                new ColumnDef
                (
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new RelType(),
                    0
                ),
                new ColumnDef
                (
                    new TypeDef("Constant", new TypeDef("Int64", 1)),
                    new RelType(),
                    0
                ),
                new ColumnDef
                (
                    new TypeDef("Constant", new TypeDef("String", "foo")),
                    new RelType(),
                    0
                ),
                new ColumnDef
                (
                    new TypeDef("String"), new RelType(), 0
                ),
                new ColumnDef
                (
                    new TypeDef("Constant", new TypeDef("String", "bar")),
                    new RelType(),
                    1
                ),
                new ColumnDef
                (
                    new TypeDef("Char"), new RelType(), 1
                ),
                new ColumnDef
                (
                    new TypeDef("Int64"), new RelType(), 2
                ),
            };

            var reader = MockRelationReader();

            for (int i = 0; i < reader.ColumnDefs.Count; i++)
            {
                Assert.Equal(reader.ColumnDefs[i].ArrowIndex, expected[i].ArrowIndex);
                Assert.Equal(reader.ColumnDefs[i].TypeDef, expected[i].TypeDef);
            }
        }

        RelationReader MockRelationReader()
        {
            var reader = new RelationReader
            {
                RelationID = MockRelationID() as RelationId,
                Table = MockArrowTable()
            };

            reader.ColumnDefs = reader.GetColumnDefsFromProtobuf();

            return reader;
        }

        IMessage MockRelationID()
        {
            var relationId = JsonParser.Default.Parse(metadataString, RelationId.Descriptor);
            return relationId;
        }

        Table MockArrowTable()
        {
            Schema.Builder builder = new Schema.Builder();
            builder.Field(new Field("v1", StringType.Default, false));
            builder.Field(new Field("v2", UInt32Type.Default, false));
            builder.Field(new Field("v3", Int64Type.Default, false));

            var recordBatch = new RecordBatch
            (
                builder.Build(),
                new List<IArrowArray>
                {
                    new StringArray.Builder().AppendRange(new List<string> { "w", "x", "y", "z" }).Build(),
                    new UInt32Array.Builder().AppendRange(new List<uint> { 97, 98, 99, 100 }).Build(),
                    new Int64Array.Builder().AppendRange(new List<long> { 1, 2, 3, 4 }).Build(),
                },
                4
             );

            return Table.TableFromRecordBatches(recordBatch.Schema, new List<RecordBatch> { recordBatch });
        }

        readonly string metadataString = @"
            {
    ""arguments"": [
        {
            ""tag"": ""CONSTANT_TYPE"",
            ""constantType"": {
                ""relType"": {
                    ""tag"": ""PRIMITIVE_TYPE"",
                    ""primitiveType"": ""STRING""
                },
                ""value"": {
                    ""arguments"": [
                        {
                            ""tag"": ""STRING"",
                            ""stringVal"": ""b3V0cHV0""
                        }
                    ]
                }
            }
        },
        {
            ""tag"": ""CONSTANT_TYPE"",
            ""constantType"": {
                ""relType"": {
                    ""tag"": ""PRIMITIVE_TYPE"",
                    ""primitiveType"": ""INT_64""
                },
                ""value"": {
                    ""arguments"": [
                        {
                            ""tag"": ""INT_64"",
                            ""int64Val"": ""1""
                        }
                    ]
                }
            }
        },
        {
            ""tag"": ""CONSTANT_TYPE"",
            ""constantType"": {
                ""relType"": {
                    ""tag"": ""PRIMITIVE_TYPE"",
                    ""primitiveType"": ""STRING""
                },
                ""value"": {
                    ""arguments"": [
                        {
                            ""tag"": ""STRING"",
                            ""stringVal"": ""Zm9v""
                        }
                    ]
                }
            }
        },
        {
            ""tag"": ""PRIMITIVE_TYPE"",
            ""primitiveType"": ""STRING""
        },
        {
            ""tag"": ""CONSTANT_TYPE"",
            ""constantType"": {
                ""relType"": {
                    ""tag"": ""PRIMITIVE_TYPE"",
                    ""primitiveType"": ""STRING""
                },
                ""value"": {
                    ""arguments"": [
                        {
                            ""tag"": ""STRING"",
                            ""stringVal"": ""YmFy""
                        }
                    ]
                }
            }
        },
        {
            ""tag"": ""PRIMITIVE_TYPE"",
            ""primitiveType"": ""CHAR""
        },
        {
            ""tag"": ""PRIMITIVE_TYPE"",
            ""primitiveType"": ""INT_64""
        }
    ]
}
        ";
    }
}
