// <copyright file="CsvOptions.cs" company="PlaceholderCompany">
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

    public class CsvOptions
    {
        public CsvOptions()
        {
        }

        public char? Delim { get; set; }

        public char? EscapeChar { get; set; }

        public int? HeaderRow { get; set; }

        public char? QuoteChar { get; set;  }

        public Dictionary<string, string> Schema { get; set; }

        public CsvOptions WithDelim(char delim)
        {
            this.Delim = delim;
            return this;
        }

        public CsvOptions WithEscapeChar(char escapeChar)
        {
            this.EscapeChar = escapeChar;
            return this;
        }

        public CsvOptions WithHeaderRow(int headerRow)
        {
            this.HeaderRow = headerRow;
            return this;
        }

        public CsvOptions WithQuoteChar(char quoteChar)
        {
            this.QuoteChar = quoteChar;
            return this;
        }

        public CsvOptions WithSchema(Dictionary<string, string> schema)
        {
            this.Schema = schema;
            return this;
        }
    }
}