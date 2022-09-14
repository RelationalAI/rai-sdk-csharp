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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;
using Apache.Arrow;
using Google.Protobuf.Collections;
using Relationalai.Protocol;
using RelationalAI.Results.Models;

namespace RelationalAI.Results
{
    public static class Utils
    {
        private const double UnixEpoch = 62135683200000;
        private const double MillisPerDay = 24 * 60 * 60 * 1000;

        public static IList ArrowRowToValues(IList row, List<ColumnDef> colDefs)
        {
            var output = new List<object>();

            foreach (var colDef in colDefs)
            {
                if (colDef.TypeDef.Type == "Constant")
                {
                    output.Add(Utils.ConvertValue(colDef.TypeDef, null));
                }
                else
                {
                    var value = Utils.ConvertValue(colDef.TypeDef, row[colDef.ArrowIndex]);
                    output.Add(value);
                }
            }

            return output;
        }

        public static List<object> UnflattenConstantValue(TypeDef typeDef, RepeatedField<PrimitiveValue> value)
        {
            return null;
        }

        public static TypeDef GetColumnDefFromProtobuf(RelType relType)
        {
            if (relType.Tag == Kind.ConstantType)
            {
                var typeDef = GetColumnDefFromProtobuf(relType.ConstantType.RelType);

                if (typeDef.Type != "ValueType")
                {
                    var values = new List<object>();
                    foreach (var argument in relType.ConstantType.Value.Arguments)
                    {
                        values.Add(MapPrimitiveValue(argument));
                    }

                    object convertedValue;
                    if (values.Count == 1)
                    {
                        convertedValue = Utils.ConvertValue(typeDef, values[0]);
                    }
                    else
                    {
                        convertedValue = Utils.ConvertValue(typeDef, values);
                    }

                    typeDef.Value = convertedValue;

                    return new TypeDef("Constant", typeDef);
                }
                else
                {
                    throw new Exception($"value type not handled");
                }
            }

            if (relType.Tag == Kind.PrimitiveType)
            {
                return relType.PrimitiveType switch
                {
                    PrimitiveType.String => new TypeDef("String"),
                    PrimitiveType.Char => new TypeDef("Char"),
                    PrimitiveType.Bool => new TypeDef("Bool"),
                    PrimitiveType.Int8 => new TypeDef("Int8"),
                    PrimitiveType.Int16 => new TypeDef("Int16"),
                    PrimitiveType.Int32 => new TypeDef("Int32"),
                    PrimitiveType.Int64 => new TypeDef("Int64"),
                    PrimitiveType.Int128 => new TypeDef("Int128"),
                    PrimitiveType.Uint8 => new TypeDef("UInt8"),
                    PrimitiveType.Uint16 => new TypeDef("UInt16"),
                    PrimitiveType.Uint32 => new TypeDef("UInt32"),
                    PrimitiveType.Uint64 => new TypeDef("UInt64"),
                    PrimitiveType.Uint128 => new TypeDef("UInt128"),
                    PrimitiveType.Float16 => new TypeDef("Float16"),
                    PrimitiveType.Float32 => new TypeDef("Float32"),
                    PrimitiveType.Float64 => new TypeDef("Float64"),
                    _ => throw new Exception($"Unhandled rel primitive type: {relType.PrimitiveType}"),
                };
            }

            if (relType.Tag == Kind.ValueType)
            {
                var typeDefs = new List<TypeDef>();

                foreach (var tp in relType.ValueType.ArgumentTypes)
                {
                    typeDefs.Add(GetColumnDefFromProtobuf(tp));
                }

                var typeDef = new TypeDef("ValueType", typeDefs: typeDefs);

                return MapValueType(typeDef);
            }

            return new TypeDef("unknown");
        }

        public static TypeDef MapValueType(TypeDef typeDef)
        {
            var slice = typeDef.TypeDefs.Count < 3 ? typeDef.TypeDefs.Count : 3;
            var relNames = new List<TypeDef>();

            foreach (var tp in typeDef.TypeDefs.GetRange(0, slice))
            {
                if (tp.Type == "Constant" && (tp.Value as TypeDef).Type == "String")
                {
                    relNames.Add(tp);
                }
            }

            if (relNames.Count != 3 ||
                !((relNames[0].Value as TypeDef).Value as string == "rel" &&
                    (relNames[1].Value as TypeDef).Value as string == "base"))
            {
                return typeDef;
            }

            var standardValueType = (relNames[2].Value as TypeDef).Value as string;
            switch (standardValueType)
            {
                case "DateTime":
                case "Date":
                case "Year":
                case "Month":
                case "Week":
                case "Day":
                case "Hour":
                case "Minute":
                case "Second":
                case "Millisecond":
                case "Microsecond":
                case "Nanosecond":
                case "FilePos":
                case "Missing":
                case "Hash":
                    return new TypeDef(standardValueType);
                case "FixedDecimal":
                    if (typeDef.TypeDefs.Count == 6 &&
                    typeDef.TypeDefs[3].Type == "Constant" &&
                        typeDef.TypeDefs[4].Type == "Constant")
                    {
                        var bits = (long)(typeDef.TypeDefs[3].Value as TypeDef).Value;
                        var places = (long)(typeDef.TypeDefs[4].Value as TypeDef).Value;

                        if (bits == 16 || bits == 32 || bits == 64 || bits == 128)
                        {
                            return new TypeDef($"Decimal{bits}", places: Convert.ToInt32(places));
                        }
                    }

                    throw new Exception($"unknow fixed decimal type definition");
                case "Rational":
                    var typeDefs = typeDef.TypeDefs;
                    if (typeDefs.Count == 5 || typeDefs.Count == 3)
                    {
                        switch (typeDefs[3].Type)
                        {
                            case "Int8":
                                return new TypeDef("Rational8");
                            case "Int16":
                                return new TypeDef("Rational16");
                            case "Int32":
                                return new TypeDef("Rational32");
                            case "Int64":
                                return new TypeDef("Rational64");
                            case "Int128":
                                return new TypeDef("Rational128");
                        }
                    }

                    throw new Exception($"unknow rational type definition");
                default:
                    throw new Exception($"unhandled standard value type: {standardValueType}");
            }
        }

        public static object MapPrimitiveValue(PrimitiveValue argument)
        {
            return argument.ValueCase switch
            {
                PrimitiveValue.ValueOneofCase.StringVal => Encoding.Default.GetString(argument.StringVal.ToByteArray()),
                PrimitiveValue.ValueOneofCase.CharVal => argument.CharVal,
                PrimitiveValue.ValueOneofCase.BoolVal => argument.BoolVal,
                PrimitiveValue.ValueOneofCase.Int8Val => argument.Int8Val,
                PrimitiveValue.ValueOneofCase.Int16Val => argument.Int16Val,
                PrimitiveValue.ValueOneofCase.Int32Val => argument.Int32Val,
                PrimitiveValue.ValueOneofCase.Int64Val => argument.Int64Val,
                PrimitiveValue.ValueOneofCase.Int128Val => new[] { argument.Int128Val.Lowbits, argument.Int128Val.Highbits },
                PrimitiveValue.ValueOneofCase.Uint8Val => argument.Uint8Val,
                PrimitiveValue.ValueOneofCase.Uint16Val => argument.Uint16Val,
                PrimitiveValue.ValueOneofCase.Uint32Val => argument.Uint32Val,
                PrimitiveValue.ValueOneofCase.Uint64Val => argument.Uint64Val,
                PrimitiveValue.ValueOneofCase.Uint128Val => new[] { argument.Uint128Val.Lowbits, argument.Uint128Val.Highbits },
                PrimitiveValue.ValueOneofCase.Float16Val => argument.Float16Val,
                PrimitiveValue.ValueOneofCase.Float32Val => argument.Float32Val,
                PrimitiveValue.ValueOneofCase.Float64Val => argument.Float64Val,
                _ => throw new Exception($"unhandled protobuf primitive value map value: {argument.ValueCase}"),
            };
        }

        public static object ConvertValue(TypeDef typeDef, object value)
        {
            switch (typeDef.Type)
            {
                case "Constant":
                    return (typeDef.Value as TypeDef).Value;
                case "String":
                    return value;
                case "Bool":
                    return value;
                case "Char":
                    return Convert.ToChar(value);
                case "DateTime":
                    return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((long)value - UnixEpoch);
                case "Date":
                    return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(((long)value * MillisPerDay) - UnixEpoch);
                case "Month":
                    return DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(Convert.ToInt32(value));
                case "Year":
                case "Day":
                case "Week":
                case "Hour":
                case "Minute":
                case "Second":
                case "Millisecond":
                case "Microsecond":
                case "Nanosecond":
                case "FilePos":
                case "Int8":
                case "Int16":
                case "Int32":
                case "Int64":
                case "UInt8":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                case "Float16":
                case "Float32":
                case "Float64":
                    return value;
                case "Int128":
                    return Utils.Int128ToBigInteger(value as ulong[]);
                case "UInt128":
                    return Utils.Uint128ToBigInteger(value as ulong[]);
                case "Decimal16":
                case "Decimal32":
                case "Decimal64":
                    return Convert.ToDecimal(value) / Convert.ToDecimal(Math.Pow(10, (double)typeDef.Places));
                case "Hash":
                    return Utils.Uint128ToBigInteger(value as ulong[]);
                case "Missing":
                    return null;
                case "Rational8":
                case "Rational16":
                case "Rational32":
                case "Rational64":
                    var num = (value as List<object>)[0];
                    var denom = (value as List<object>)[1];
                    return $"{num} / {denom}";
                case "Rational128":
                    var numValues = (value as List<object>)[0] as ulong[];
                    var denomValues = (value as List<object>)[1] as ulong[];
                    return $"{Utils.Int128ToBigInteger(numValues)} / {Utils.Int128ToBigInteger(denomValues)}";
                case "ValueType":
                    var physicalIndex = -1;
                    var values = new List<object>();
                    foreach (var tp in typeDef.TypeDefs)
                    {
                        if (tp.Type == "Constant")
                        {
                            values.Add(ConvertValue(tp, null));
                        }
                        else
                        {
                            physicalIndex++;
                            if (value is IList)
                            {
                                values.Add(ConvertValue(tp, (value as List<object>)[physicalIndex]));
                            }
                            else
                            {
                                values.Add(ConvertValue(tp, value));
                            }
                        }
                    }

                    return values.ToArray();
                default:
                    throw new Exception($"unhandled convert value type: {typeDef.Type}");
            }
        }

        public static IList ArrowArrayToArray(Apache.Arrow.Array array)
        {
            var output = new List<object>();

            switch (array.GetType().Name)
            {
                case "UInt8Array":
                    for (int i = 0; i < (array as UInt8Array).Length; i++)
                    {
                        output.Add((array as UInt8Array).GetValue(i).Value);
                    }

                    break;
                case "UInt16Array":
                    for (int i = 0; i < (array as UInt16Array).Length; i++)
                    {
                        output.Add((array as UInt16Array).GetValue(i).Value);
                    }

                    break;
                case "UInt32Array":
                    for (int i = 0; i < (array as UInt32Array).Length; i++)
                    {
                        output.Add((array as UInt32Array).GetValue(i).Value);
                    }

                    break;
                case "UInt64Array":
                    for (int i = 0; i < (array as UInt64Array).Length; i++)
                    {
                        output.Add((array as UInt64Array).GetValue(i).Value);
                    }

                    break;
                case "Int8Array":
                    for (int i = 0; i < (array as Int8Array).Length; i++)
                    {
                        output.Add((array as Int8Array).GetValue(i).Value);
                    }

                    break;
                case "Int16Array":
                    for (int i = 0; i < (array as Int16Array).Length; i++)
                    {
                        output.Add((array as Int16Array).GetValue(i).Value);
                    }

                    break;
                case "Int32Array":
                    for (int i = 0; i < (array as Int32Array).Length; i++)
                    {
                        output.Add((array as Int32Array).GetValue(i).Value);
                    }

                    break;
                case "Int64Array":
                    for (int i = 0; i < (array as Int64Array).Length; i++)
                    {
                        output.Add((array as Int64Array).GetValue(i).Value);
                    }

                    break;
                case "FloatArray":
                    for (int i = 0; i < (array as FloatArray).Length; i++)
                    {
                        output.Add((array as FloatArray).GetValue(i).Value);
                    }

                    break;
                case "DoubleArray":
                    for (int i = 0; i < (array as DoubleArray).Length; i++)
                    {
                        output.Add((array as DoubleArray).GetValue(i).Value);
                    }

                    break;
                case "StringArray":
                    for (int i = 0; i < (array as StringArray).Length; i++)
                    {
                        output.Add((array as StringArray).GetString(i));
                    }

                    break;
                case "BooleanArray":
                    for (int i = 0; i < (array as BooleanArray).Length; i++)
                    {
                        output.Add((array as BooleanArray).GetValue(i).Value);
                    }

                    break;
                case "StructArray":
                    var inner = new List<object>();

                    foreach (var field in (array as StructArray).Fields)
                    {
                        inner.AddRange(ArrowArrayToArray(field as Apache.Arrow.Array) as List<object>);
                    }

                    output.Add(inner);
                    break;
                default:
                    throw new System.Exception($"unhandled arrow array type: {array.GetType().Name}");
            }

            return output;
        }

        public static bool IsFullySpecialized(List<ColumnDef> colDefs)
        {
            if (colDefs.Count == 0)
            {
                return false;
            }

            foreach (var colDef in colDefs)
            {
                if (colDef.TypeDef.Type != "Constant")
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsSpecialized(ColumnDef colDef)
        {
            if (colDef.TypeDef.Type != "Constant")
            {
                return false;
            }

            return true;
        }

        public static BigInteger Uint128ToBigInteger(ulong[] values)
        {
            var lowBits = values[0];
            var highBits = values[1];

            return (new BigInteger(BitConverter.GetBytes(highBits), isUnsigned: true) << 64) | lowBits;
        }

        public static BigInteger Int128ToBigInteger(ulong[] values)
        {
            var lowBits = values[0];
            var highBits = values[1];

            return (new BigInteger(BitConverter.GetBytes(highBits), isUnsigned: false) << 64) | lowBits;
        }
    }
}
