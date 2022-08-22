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

namespace RelationalAI.Model.Transaction
{
    public class CsvOptions
    {
        public char? Delim { get; private set; }

        public char? EscapeChar { get; private set; }

        public int? HeaderRow { get; private set; }

        public char? QuoteChar { get; private set; }

        public Dictionary<string, string> Schema { get; private set; }

        public CsvOptions()
        {
        }

        public CsvOptions WithDelim(char delim)
        {
            Delim = delim;
            return this;
        }

        public CsvOptions WithEscapeChar(char escapeChar)
        {
            EscapeChar = escapeChar;
            return this;
        }

        public CsvOptions WithHeaderRow(int headerRow)
        {
            HeaderRow = headerRow;
            return this;
        }

        public CsvOptions WithQuoteChar(char quoteChar)
        {
            QuoteChar = quoteChar;
            return this;
        }

        public CsvOptions WithSchema(Dictionary<string, string> schema)
        {
            Schema = schema;
            return this;
        }
    }
}