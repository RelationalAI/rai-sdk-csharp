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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using RelationalAI.Results;
using RelationalAI.Results.Models;
using Newtonsoft.Json;
using Xunit;

namespace RelationalAI.Test
{
    class Test
    {
        public Test(string name, string query, List<TypeDef> typeDefs, List<object> values, bool skip)
        {
            Name = name;
            Query = query;
            TypeDefs = typeDefs;
            Values = values;
            Skip = skip;
        }

        public string Name { get; set; }
        public string Query { get; set; }
        public List<TypeDef> TypeDefs { get; set; }
        public List<object> Values { get; set; }
        public bool Skip { get; set; }
    }

    public class RelationReaderIntegrationTest : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string DBname = $"csharp-sdk-{Uuid}";
        public static string Engine = $"csharp-sdk-{Uuid}";

        [Fact]
        public async Task StandardTypeIntegrationTestsAsync()
        {
            var client = CreateClient();

            await client.CreateEngineWaitAsync(Engine);
            await client.CreateDatabaseAsync(DBname, Engine);

            foreach(var test in standardTypeTests)
            {
                if (!test.Skip)
                {
                    Console.WriteLine($"Test: {test.Name}");

                    var rsp = await client.ExecuteWaitAsync(DBname, Engine, test.Query, true);
                    Assert.NotNull(rsp);

                    var reader = new RelationReader(rsp.Results[0]);
                    reader.Print();
                    var typeDefs = reader.TypeDefs();
                    var tuple = reader.Tuple(0);

                    Assert.Equal(test.TypeDefs, typeDefs);
                    Assert.Equal(test.Values, tuple);
                }
            }
        }

        [Fact]
        public async Task SpecializationTypeIntegrationTestsAsync()
        {
            var client = CreateClient();

            await client.CreateEngineWaitAsync(Engine);
            await client.CreateDatabaseAsync(DBname, Engine);

            foreach (var test in specializationTypeTests)
            {
                if (!test.Skip)
                {
                    Console.WriteLine($"Test: {test.Name}");

                    var rsp = await client.ExecuteWaitAsync(DBname, Engine, test.Query, true);
                    Assert.NotNull(rsp);

                    var reader = new RelationReader(rsp.Results[0]);
                    reader.Print();
                    var typeDefs = reader.TypeDefs();
                    var tuple = reader.Tuple(0);

                    Assert.Equal(test.TypeDefs, typeDefs);
                    Assert.Equal(test.Values, tuple);
                }
            }
        }

        [Fact]
        public async Task MiscValueTypeIntegrationTestsAsync()
        {
            var client = CreateClient();

            await client.CreateEngineWaitAsync(Engine);
            await client.CreateDatabaseAsync(DBname, Engine);

            foreach(var test in miscValueTypeTests)
            {
                if (!test.Skip)
                {
                    Console.WriteLine($"Test: {test.Name}");

                    var rsp = await client.ExecuteWaitAsync(DBname, Engine, test.Query, true);
                    Assert.NotNull(rsp);

                    var reader = new RelationReader(rsp.Results[0]);
                    reader.Print();
                    var typeDefs = reader.TypeDefs();
                    var tuple = reader.Tuple(0);

                    Assert.Equal(JsonConvert.SerializeObject(test.TypeDefs), JsonConvert.SerializeObject(typeDefs));
                    Assert.Equal(test.Values, tuple);
                }
            }
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try { await client.DeleteDatabaseAsync(DBname); } catch { }
            try { await client.DeleteEngineWaitAsync(Engine); } catch { }
        }

        readonly List<Test> standardTypeTests = new List<Test>
        {
            new Test
            (
                "String",
                @"def output = ""test""",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("String")
                },
                new List<object> {"output", "test" },
                false
            ),
            new Test
            (
                "Bool",
                @"def output = boolean_true, boolean_false",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Bool"),
                    new TypeDef("Bool"),
                },
                new List<object> {"output", true, false },
                false
            ),
            new Test
            (
                "Char",
                @"def output = 'a', '?'",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Char"),
                    new TypeDef("Char"),
                },
                new List<object> {"output", 'a', '?'},
                false
            ),
            new Test
            (
                "DateTime",
                @"def output = 2021-10-12T01:22:31+10:00",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("DateTime"),
                },
                new List<object> {"output", DateTime.Parse("2021-10-11T15:22:31Z").ToUniversalTime()},
                false
            ),
            new Test
            (
                "Date",
                @"def output = 2021-10-12",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Date"),
                },
                new List<object> {"output", DateTime.Parse("2021-10-12Z").ToUniversalTime()},
                false
            ),
            new Test
            (
                "Year",
                @"def output = Year[2022]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Year"),
                },
                new List<object> {"output", Convert.ToInt64(2022)},
                false
            ),
            new Test
            (
                "Month",
                @"def output = Month[1]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Month"),
                },
                new List<object> {"output", DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(1)},
                false
            ),
            new Test
            (
                "Week",
                @"def output = Week[1]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Week"),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Day",
                @"def output = Day[1]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Day"),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Hour",
                @"def output = Hour[1]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Hour"),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Minute",
                @"def output = Minute[1]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Minute"),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Second",
                @"def output = Second[1]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Second"),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Millisecond",
                @"def output = Millisecond[1]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Millisecond"),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Microsecond",
                @"def output = Microsecond[1]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Microsecond"),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Nanosecond",
                @"def output = Nanosecond[1]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Nanosecond"),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new Test
            (
                "Hash",
                @"
                    entity type Foo = Int
                    def output = ^Foo[12]
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Hash"),
                },
                null,
                true
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new Test
            (
                "Missing",
                @"def output = missing",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Missing"),
                },
                null,
                true
            ),
            new Test
            (
                "FilePos",
                @"
                def config:data = """"""
                a,b,c
                1,2,3
                4,5,6
                """"""

                def csv = load_csv[config]

                def output(p) = csv(_, p, _)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("FilePos"),
                },
                new List<object> {"output", Convert.ToInt64(2)},
                false
            ),
            new Test
            (
                "Int8",
                @"def output = int[8, 12], int[8, -12]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Int8"),
                    new TypeDef("Int8"),
                },
                new List<object> {"output", Convert.ToSByte(12), Convert.ToSByte(-12)},
                false
            ),
            new Test
            (
                "Int16",
                @"def output = int[16, 123], int[16, -123]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Int16"),
                    new TypeDef("Int16"),
                },
                new List<object> {"output", Convert.ToInt16(123), Convert.ToInt16(-123)},
                false
            ),
            new Test
            (
                "Int32",
                @"def output = int[32, 1234], int[32, -1234]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Int32"),
                    new TypeDef("Int32"),
                },
                new List<object> {"output", Convert.ToInt32(1234), Convert.ToInt32(-1234)},
                false
            ),
            new Test
            (
                "Int64",
                @"def output = int[64, 12345], int[64, -12345]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Int64"),
                    new TypeDef("Int64"),
                },
                new List<object> {"output", Convert.ToInt64(12345), Convert.ToInt64(-12345)},
                false
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new Test
            (
                "Int128",
                @"def output = 123456789101112131415, int[128, 0], int[128, -10^10]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Int128"),
                    new TypeDef("Int128"),
                    new TypeDef("Int128"),
                },
                null,
                true
            ),
            new Test
            (
                "UInt8",
                @"def output = uint[8, 12]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("UInt8"),
                },
                new List<object> {"output", Convert.ToByte(12)},
                false
            ),
            new Test
            (
                "UInt16",
                @"def output = uint[16, 123]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("UInt16"),
                },
                new List<object> {"output", Convert.ToUInt16(123)},
                false
            ),
            new Test
            (
                "UInt32",
                @"def output = uint[32, 1234]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("UInt32"),
                },
                new List<object> {"output", Convert.ToUInt32(1234)},
                false
            ),
            new Test
            (
                "UInt64",
                @"def output = uint[64, 12345]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("UInt64"),
                },
                new List<object> {"output", Convert.ToUInt64(12345)},
                false
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new Test
            (
                "Int128",
                @"def output = uint[128, 123456789101112131415], uint[128, 0], 0xdade49b564ec827d92f4fd30f1023a1e",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Int128"),
                    new TypeDef("Int128"),
                    new TypeDef("Int128"),
                },
                null,
                true
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/30
            new Test
            (
                "Float16",
                @"def output = float[16, 12], float[16, 42.5]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Float16"),
                    new TypeDef("Float16"),
                },
                new List<object> {"output", 12, 42.5},
                true
            ),
            new Test
            (
                "Float32",
                @"def output = float[32, 12], float[32, 42.5]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Float32"),
                    new TypeDef("Float32"),
                },
                new List<object> {"output", Convert.ToSingle(12), Convert.ToSingle(42.5)},
                false
            ),
            new Test
            (
                "Float64",
                @"def output = float[64, 12], float[64, 42.5]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Float64"),
                    new TypeDef("Float64"),
                },
                new List<object> {"output", Convert.ToDouble(12), Convert.ToDouble(42.5)},
                false
            ),
            new Test
            (
                "Decimal16",
                @"def output = parse_decimal[16, 2, ""12.34""]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Decimal16", places: 2),
                },
                new List<object> {"output", 12.34m},
                false
            ),
            new Test
            (
                "Decimal32",
                @"def output = parse_decimal[32, 2, ""12.34""]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Decimal32", places: 2),
                },
                new List<object> {"output", 12.34m},
                false
            ),
            new Test
            (
                "Decimal64",
                @"def output = parse_decimal[64, 2, ""12.34""]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Decimal64", places: 2),
                },
                new List<object> {"output", 12.34m},
                false
            ),
            // FIXME
            new Test
            (
                "Decimal64",
                @"def output = parse_decimal[64, 2, ""12345678901011121314.34""]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Decimal64", places: 2),
                },
                new List<object> {"output", 12345678901011121314.34m},
                true
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/30
            new Test
            (
                "Rational8",
                @"def output = rational[8, 1, 2]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Rational8"),
                },
                null,
                true
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/30
            new Test
            (
                "Rational16",
                @"def output = rational[16, 1, 2]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Rational16"),
                },
                null,
                true
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/30
            new Test
            (
                "Rational32",
                @"def output = rational[32, 1, 2]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Rational32"),
                },
                null,
                true
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/30
            new Test
            (
                "Rational64",
                @"def output = rational[64, 1, 2]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Rational64"),
                },
                null,
                true
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/30
            new Test
            (
                "Rational128",
                @"def output = rational[128, 123456789101112313, 9123456789101112313]",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Rational128"),
                },
                null,
                true
            ),
        };

        readonly List<Test> specializationTypeTests = new List<Test>
        {
            new Test
            (
                "String(symbol)",
                @"def output = :foo",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("String", "foo")),
                },
                new List<object> {"output", "foo"},
                false
            ),
            new Test
            (
                "String",
                @"
                def v = ""foo""
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("String", "foo")),
                },
                new List<object> {"output", "foo"},
                false
            ),
            new Test
            (
                "String with slash",
                @"
                def v = ""foo / bar""
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("String", "foo / bar")),
                },
                new List<object> {"output", "foo / bar"},
                false
            ),
            new Test
            (
                "Char",
                @"
                def v = '?'
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Char", '?')),
                },
                new List<object> {"output", '?'},
                false
            ),
            // FIXME: DateTime precision
            new Test
            (
                "DateTime",
                @"
                def v = 2021-10-12T01:22:31+10:00
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("DateTime", DateTime.Parse("2022-10-11T15:22:31Z").ToUniversalTime())),
                },
                new List<object> {"output", DateTime.Parse("2022-10-11T15:22:31Z").ToUniversalTime()},
                false
            ),
            new Test
            (
                "Date",
                @"
                def v = 2021-10-12
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Date", DateTime.Parse("2021-10-12Z").ToUniversalTime())),
                },
                new List<object> {"output", DateTime.Parse("2021-10-12Z").ToUniversalTime()},
                false
            ),
            new Test
            (
                "Year",
                @"
                def v = Year[2022]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Year", Convert.ToInt64(2022))),
                },
                new List<object> {"output", Convert.ToInt64(2022)},
                false
            ),
            new Test
            (
                "Month",
                @"
                def v = Month[1]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Month", DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(1))),
                },
                new List<object> {"output", DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(1)},
                false
            ),
            new Test
            (
                "Week",
                @"
                def v = Week[1]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Week", Convert.ToInt64(1))),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Day",
                @"
                def v = Day[1]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Day", Convert.ToInt64(1))),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Hour",
                @"
                def v = Hour[1]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Hour", Convert.ToInt64(1))),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Minute",
                @"
                def v = Minute[1]
                def output = #(v)
                "
                ,
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Minute", Convert.ToInt64(1))),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Second",
                @"
                def v = Second[1]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Second", Convert.ToInt64(1))),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Millisecond",
                @"
                def v = Millisecond[1]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Millisecond", Convert.ToInt64(1))),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Microsecond",
                @"
                def v = Microsecond[1]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Microsecond", Convert.ToInt64(1))),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Nanosecond",
                @"
                def v = Nanosecond[1]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Nanosecond", Convert.ToInt64(1))),
                },
                new List<object> {"output", Convert.ToInt64(1)},
                false
            ),
            new Test
            (
                "Hash",
                @"
                    entity type Foo = Int
                    def v = ^Foo[12]
                    def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Hash", BigInteger.Parse("290925887971139297379988470542779955742"))),
                },
                new List<object> {"output", BigInteger.Parse("290925887971139297379988470542779955742")},
                false
            ),
            new Test
            (
                "Missing",
                @"
                def v = missing
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Missing", null)),
                },
                new List<object> {"output", null},
                false
            ),
            new Test
            (
                "FilePos",
                @"
                def config:data = """"""
                a,b,c
                1,2,3
                4,5,6
                """"""

                def csv = load_csv[config]

                def v(p) = csv(_, p, _)
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("FilePos", Convert.ToInt64(2))),
                },
                new List<object> {"output", Convert.ToInt64(2)},
                false
            ),
            new Test
            (
                "Int8",
                @"
                def v = int[8, 12]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Int8", Convert.ToInt32(12))),
                },
                new List<object> {"output", Convert.ToInt32(12)},
                false
            ),
            new Test
            (
                "Int16",
                @"
                def v = int[16, 123]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Int16", Convert.ToInt32(123))),
                },
                new List<object> {"output", Convert.ToInt32(123)},
                false
            ),
            new Test
            (
                "Int32",
                @"
                def v = int[32, 1234]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Int32", Convert.ToInt32(1234))),
                },
                new List<object> {"output", Convert.ToInt32(1234)},
                false
            ),
            new Test
            (
                "Int64",
                @"
                def v = int[64, -12345]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Int64", Convert.ToInt64(-12345))),
                },
                new List<object> {"output", Convert.ToInt64(-12345)},
                false
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new Test
            (
                "Int128",
                @"
                def v = int[128, -123456789101112131415]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Int128", BigInteger.Parse("-123456789101112131415"))),
                },
                new List<object> {"output", BigInteger.Parse("-123456789101112131415")},
                false
            ),
            new Test
            (
                "UInt8",
                @"
                def v = uint[8, 12]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("UInt8", Convert.ToUInt32(12))),
                },
                new List<object> {"output", Convert.ToUInt32(12)},
                false
            ),
            new Test
            (
                "UInt16",
                @"
                def v = uint[16, 123]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("UInt16", Convert.ToUInt32(123))),
                },
                new List<object> {"output", Convert.ToUInt32(123)},
                false
            ),
            new Test
            (
                "UInt32",
                @"
                def v = uint[32, 1234]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("UInt32", Convert.ToUInt32(1234))),
                },
                new List<object> {"output", Convert.ToUInt32(1234)},
                false
            ),
            new Test
            (
                "UInt64",
                @"
                def v = uint[64, 12345]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("UInt64", Convert.ToUInt64(12345))),
                },
                new List<object> {"output", Convert.ToUInt64(12345)},
                false
            ),
            new Test
            (
                "UInt128",
                @"
                def v = uint[128, 123456789101112131415]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("UInt128", BigInteger.Parse("123456789101112131415"))),
                },
                new List<object> {"output", BigInteger.Parse("123456789101112131415")},
                false
            ),
            new Test
            (
                "Float16",
                @"
                def v = float[16, 42.5]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Float16", Convert.ToSingle(42.5))),
                },
                new List<object> {"output", Convert.ToSingle(42.5)},
                false
            ),
            new Test
            (
                "Float32",
                @"
                def v = float[32, 42.5]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Float32", Convert.ToSingle(42.5))),
                },
                new List<object> {"output", Convert.ToSingle(42.5)},
                false
            ),
            new Test
            (
                "Float64",
                @"
                def v = float[64, 42.5]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Float64", Convert.ToDouble(42.5))),
                },
                new List<object> {"output", Convert.ToDouble(42.5)},
                false
            ),
            new Test
            (
                "Decimal16",
                @"
                def v = parse_decimal[16, 2, ""12.34""]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Decimal16", 12.34m, 2)),
                },
                new List<object> {"output", 12.34m},
                false
            ),
            new Test
            (
                "Decimal32",
                @"
                def v = parse_decimal[32, 2, ""12.34""]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Decimal32", 12.34m, 2)),
                },
                new List<object> {"output", 12.34m},
                false
            ),
            new Test
            (
                "Decimal64",
                @"
                def v = parse_decimal[64, 2, ""12.34""]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Decimal64", 12.34m, 2)),
                },
                new List<object> {"output", 12.34m},
                false
            ),
            // FIXME
            new Test
            (
                "Decimal64",
                @"
                def v = parse_decimal[64, 2, ""12345678901011121314.34""]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Decimal64", 12345678901011121314.34m, 2)),
                },
                new List<object> {"output", 12345678901011121314.34m},
                true
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/30
            new Test
            (
                "Rational8",
                @"
                def v = rational[8, 1, 2]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Rational8", "1 / 2")),
                },
                new List<object> {"output", "1 / 2"},
                false
            ),
            new Test
            (
                "Rational16",
                @"
                def v = rational[16, 1, 2]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Rational16", "1 / 2")),
                },
                new List<object> {"output", "1 / 2"},
                false
            ),
            new Test
            (
                "Rational32",
                @"
                def v = rational[32, 1, 2]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Rational32", "1 / 2")),
                },
                new List<object> {"output", "1 / 2"},
                false
            ),
            new Test
            (
                "Rational64",
                @"
                def v = rational[64, 1, 2]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Rational64", "1 / 2")),
                },
                new List<object> {"output", "1 / 2"},
                false
            ),
            new Test
            (
                "Rational128",
                @"
                def v = rational[128, 123456789101112313, 9123456789101112313]
                def output = #(v)
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef("Constant", new TypeDef("Rational128", "123456789101112313 / 9123456789101112313")),
                },
                new List<object> {"output", "123456789101112313 / 9123456789101112313"},
                false
            ),
        };

        readonly List<Test> miscValueTypeTests = new List<Test>
        {
            new Test
            (
                "Int",
                @"
                value type MyType = Int
		        def output = ^MyType[123]
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef(
                        "ValueType",
                        typeDefs: new List<TypeDef>
                            {
                                new TypeDef("Constant", new TypeDef("String", "MyType")),
                                new TypeDef("Int64")
                            }
                        ),
                },
                new List<object> {"output", new object[]{"MyType", Convert.ToInt64(123)}},
                false
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/30
            new Test
            (
                "Int128",
                @"
                value type MyType = SignedInt[128]
                def output = ^MyType[123445677777999999999]
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef(
                        "ValueType",
                        typeDefs: new List<TypeDef>
                            {
                                new TypeDef("Constant", new TypeDef("String", "MyType")),
                                new TypeDef("Int128")
                            }
                        ),
                },
                new List<object> {"output", new object[]{"MyType", BigInteger.Parse("123445677777999999999")}},
                true
            ),
            new Test
            (
                "Date",
                @"
                value type MyType = Date
                def output = ^MyType[2021-10-12]
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef(
                        "ValueType",
                        typeDefs: new List<TypeDef>
                            {
                                new TypeDef("Constant", new TypeDef("String", "MyType")),
                                new TypeDef("Date")
                            }
                        ),
                },
                new List<object> {"output", new object[]{"MyType", DateTime.Parse("2021-10-12Z").ToUniversalTime()} },
                false
            ),
            new Test
            (
                "OuterType(InnerType(Int, String), String)",
                @"
                value type InnerType = Int, String
                value type OuterType = InnerType, String
                def output = ^OuterType[^InnerType[123, ""inner""], ""outer""]
                ",
                new List<TypeDef>
                {
                    new TypeDef("Constant", new TypeDef("String", "output")),
                    new TypeDef(
                        "ValueType",
                        typeDefs: new List<TypeDef>
                            {
                                new TypeDef("Constant", new TypeDef("String", "OuterType")),
                                new TypeDef(
                                    "ValueType",
                                    typeDefs: new List<TypeDef>
                                    {
                                        new TypeDef("Constant", new TypeDef("String", "InnerType")),
                                        new TypeDef("Int64"),
                                        new TypeDef("String"),
                                    }
                                ),
                                new TypeDef("String")
                            }
                        ),
                },
                new List<object> {"output", new object[] { "OuterType", new object[] {"InnerType", Convert.ToInt64(123), "inner"}, "outer"} },
                false
            ),
        };
    }
}
