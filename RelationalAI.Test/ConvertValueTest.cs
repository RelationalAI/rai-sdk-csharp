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
using System.Linq;
using RelationalAI.Results.Models;
using Xunit;

namespace RelationalAI.Test
{
    class TestInput
    {
        public TestInput(string relType, string type, string query, object[] arrowValues, object[] values, bool skip, int? places = null)
        {
            RelType = relType;
            Type = type;
            Query = query;
            ArrowValues = arrowValues.ToList();
            Values = values.ToList();
            Skip = skip;
            Places = places;
        }

        public string RelType { get; set; }
        public string Type { get; set; }
        public string Query { get; set; }
        public List<object> ArrowValues { get; set; }
        public List<object> Values { get; set; }
        public bool Skip { get; set; }
        public int? Places { get; set; }
    }

    public class ConvertValueTest : UnitTest
    {
        [Fact]
        public void ConvertValueTests()
        {
            foreach (var test in testInputs)
            {
                if (!test.Skip)
                {
                    var typeDef = new TypeDef(test.Type);
                    if (test.Places != null)
                    {
                        typeDef.Places = test.Places;
                    }

                    for (int i = 0; i < test.ArrowValues.Count; i++)
                    {
                        var value = test.ArrowValues[i];
                        var convertedValue = Results.Utils.ConvertValue(typeDef, value);
                        Assert.Equal(test.Values[i], convertedValue);
                    }
                }
            }
        }

        readonly TestInput[] testInputs =
        {
            new TestInput
            (
                "String",
                "String",
                "def output = \"test\"",
                new object[]{"test"},
                new object[]{"test"},
                false
            ),
            new TestInput
            (
                "Bool",
                "Bool",
                "def output = boolean_true, boolean_false",
                new object[]{true, false},
                new object[]{true, false},
                false
            ),
            // FixMe: fails for utf-8 chars
            // check using 128077
            new TestInput
            (
                "Char",
                "Char",
                "def output = 'a', 'z'",
                new object[]{97, 122},
                new object[]{'a', 'z'},
                false
            ),
            new TestInput
            (
                "Dates.DateTime",
                "DateTime",
                "def output = 2021-10-12T01:22:31+10:00",
                new object[]{63769648951000},
                new object[]{ DateTime.Parse("2021-10-11T15:22:31Z").ToUniversalTime() },
                false
            ),
            new TestInput
            (
                "Dates.Date",
                "Date",
                "def output = 2021-10-12",
                new object[]{738075L},
                new object[]{ DateTime.Parse("2021-10-12T00:00:00Z").ToUniversalTime() },
                false
            ),
             new TestInput
            (
                "Dates.Year",
                "Year",
                "def output = Year[2022]",
                new object[]{2022L},
                new object[]{2022L},
                false
            ),
            new TestInput
            (
                "Dates.Month",
                "Month",
                "def output = Month[1]",
                new object[]{1},
                new object[]{DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(1)},
                false
            ),
            new TestInput
            (
                "Dates.Week",
                "Week",
                "def output = Week[1]",
                new object[]{1},
                new object[]{1},
                false
            ),
            new TestInput
            (
                "Dates.Day",
                "Day",
                "def output = Day[1]",
                new object[]{1},
                new object[]{1},
                false
            ),
            new TestInput
            (
                "Dates.Hour",
                "Hour",
                "def output = Hour[1]",
                new object[]{1},
                new object[]{1},
                false
            ),
            new TestInput
            (
                "Dates.Minute",
                "Minute",
                "def output = Minute[1]",
                new object[]{1},
                new object[]{1},
                false
            ),
            new TestInput
            (
                "Dates.Second",
                "Second",
                "def output = Second[1]",
                new object[]{1},
                new object[]{1},
                false
            ),
            new TestInput
            (
                "Dates.Millisecond",
                "Millisecond",
                "def output = Millisecond[1]",
                new object[]{1},
                new object[]{1},
                false
            ),
            new TestInput
            (
                "Dates.Microsecond",
                "Microsecond",
                "def output = Microsecond[1]",
                new object[]{1},
                new object[]{1},
                false
            ),
            new TestInput
            (
                "Dates.Nanosecond",
                "Nanosecond",
                "def output = Nanosecond[1]",
                new object[]{1},
                new object[]{1},
                false
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new TestInput
            (
                "HashValue",
                "Hash",
                @"
                entity type Foo = Int
                def output = ^Foo[12]
                ",
                new object[]{1},
                new object[]{1},
                true
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new TestInput
            (
                "Missing",
                "Missing",
                @"def output = missing",
                new object[]{1},
                new object[]{1},
                true
            ),
            new TestInput
            (
                "FilePos",
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
                new object[]{2},
                new object[]{2},
                false
            ),
            new TestInput
            (
                "Int8",
                "Int8",
                @"def output = int[8, 12], int[8, -12]",
                new object[]{Convert.ToSByte(12), Convert.ToSByte(-12)},
                new object[]{Convert.ToSByte(12), Convert.ToSByte(-12)},
                false
            ),
            new TestInput
            (
                "Int16",
                "Int16",
                @"def output = int[16, 12], int[16, -12]",
                new object[]{Convert.ToInt16(12), Convert.ToInt16(-12)},
                new object[]{Convert.ToInt16(12), Convert.ToInt16(-12)},
                false
            ),
            new TestInput
            (
                "Int32",
                "Int32",
                @"def output = int[32, 12], int[32, -12]",
                new object[]{Convert.ToInt32(12), Convert.ToInt32(-12)},
                new object[]{Convert.ToInt32(12), Convert.ToInt32(-12)},
                false
            ),
            new TestInput
            (
                "Int64",
                "Int64",
                @"def output = int[64, 12], int[64, -12]",
                new object[]{Convert.ToInt64(12), Convert.ToInt64(-12)},
                new object[]{Convert.ToInt64(12), Convert.ToInt64(-12)},
                false
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new TestInput
            (
                "Int128",
                "Int128",
                @"def output = 123456789101112131415, int[128, 0], int[128, -10^10]",
                new object[]{Convert.ToInt64(12), Convert.ToInt64(-12)},
                new object[]{Convert.ToInt64(12), Convert.ToInt64(-12)},
                true
            ),
                        new TestInput
            (
                "UInt8",
                "UInt8",
                @"def output = uint[8, 12]",
                new object[]{Convert.ToByte(12)},
                new object[]{Convert.ToByte(12)},
                false
            ),
            new TestInput
            (
                "UInt16",
                "UInt16",
                @"def output = uint[16, 12]",
                new object[]{Convert.ToUInt16(12)},
                new object[]{Convert.ToUInt16(12)},
                false
            ),
            new TestInput
            (
                "UInt32",
                "UInt32",
                @"def output = uint[32, 12]",
                new object[]{Convert.ToInt32(12), Convert.ToInt32(-12)},
                new object[]{Convert.ToInt32(12), Convert.ToInt32(-12)},
                false
            ),
            new TestInput
            (
                "UInt64",
                "UInt64",
                @"def output = uint[64, 12]",
                new object[]{Convert.ToUInt64(12)},
                new object[]{Convert.ToUInt64(12)},
                false
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new TestInput
            (
                "UInt128",
                "UInt128",
                @"def output = 123456789101112131415, int[128, 0], int[128, -10^10]",
                new object[]{Convert.ToInt64(12), Convert.ToInt64(-12)},
                new object[]{Convert.ToInt64(12), Convert.ToInt64(-12)},
                true
            ),
            //FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/30
            new TestInput
            (
                "Float16",
                "Float16",
                @"def output = float[16, 12], float[16, 42.5]",
                new object[]{Convert.ToInt64(12), Convert.ToInt64(-12)},
                new object[]{Convert.ToInt64(12), Convert.ToInt64(-12)},
                true
            ),
            new TestInput
            (
                "Float32",
                "Float32",
                @"def output = float[32, 12], float[32, 42.5]",
                new object[]{12.0, 42.5},
                new object[]{12.0, 42.5},
                false
            ),
            new TestInput
            (
                "FixedPointDecimals.FixedDecimal{Int16, 2}",
                "Decimal16",
                @"def output = parse_decimal[16, 2, ""12.34""]",
                new object[]{1234},
                new object[]{12.34m},
                false,
                2
            ),
            new TestInput
            (
                "FixedPointDecimals.FixedDecimal{Int32, 2}",
                "Decimal32",
                @"def output = parse_decimal[32, 2, ""12.34""]",
                new object[]{1234},
                new object[]{12.34m},
                false,
                2
            ),
            new TestInput
            (
                "FixedPointDecimals.FixedDecimal{Int64, 2}",
                "Decimal64",
                @"def output = parse_decimal[64, 2, ""12.34""]",
                new object[]{1234},
                new object[]{12.34m},
                false,
                2
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new TestInput
            (
                "FixedPointDecimals.FixedDecimal{Int128, 2}",
                "Decimal28",
                @"def output = parse_decimal[128, 2, ""12.34""]",
                new object[]{1234},
                new object[]{12.34m},
                true,
                2
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new TestInput
            (
                "Rational{Int8}",
                "Rational8",
                @"def output = rational[8, 1, 2]",
                new object[]{1234},
                new object[]{12.34m},
                true,
                2
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new TestInput
            (
                "Rational{Int16}",
                "Rational16",
                @"def output = rational[16, 1, 2]",
                new object[]{1234},
                new object[]{12.34m},
                true,
                2
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new TestInput
            (
                "Rational{Int32}",
                "Rational32",
                @"def output = rational[32, 1, 2]",
                new object[]{1234},
                new object[]{12.34m},
                true,
                2
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new TestInput
            (
                "Rational{Int64}",
                "Rational64",
                @"def output = rational[64, 1, 2]",
                new object[]{1234},
                new object[]{12.34m},
                true,
                2
            ),
            // FIXME: https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            new TestInput
            (
                "Rational{Int128}",
                "Rational128",
                @"def output = rational[128, 1, 2]",
                new object[]{1234},
                new object[]{12.34m},
                true,
                2
            ),
        };
    }
}
