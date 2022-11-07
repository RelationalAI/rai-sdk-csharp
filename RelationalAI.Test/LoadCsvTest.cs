using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class LoadCsvTests : UnitTest
    {
        public static string Dbname = $"csharp-sdk-{Uuid}";

        private const string Sample = "" +
                                      "cocktail,quantity,price,date\n" +
                                      "\"martini\",2,12.50,\"2020-01-01\"\n" +
                                      "\"sazerac\",4,14.25,\"2020-02-02\"\n" +
                                      "\"cosmopolitan\",4,11.00,\"2020-03-03\"\n" +
                                      "\"bellini\",3,12.25,\"2020-04-04\"\n";

        [Fact]
        public async Task LoadCsvtTest()
        {
            var client = CreateClient();

            var createRsp = await CreateEngineWaitAsync(client);
            await client.CreateDatabaseAsync(Dbname, createRsp.Name);

            var loadRsp = await client.LoadCsvAsync(Dbname, createRsp.Name, "sample", Sample);
            Assert.False(loadRsp.Aborted);
            Assert.Empty(loadRsp.Output);
            Assert.Empty(loadRsp.Problems);

            var rsp = await client.ExecuteV1Async(Dbname, createRsp.Name, "def output = sample");

            var rel = FindRelation(rsp.Output, ":date");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"2020-01-01", "2020-02-02", "2020-03-03", "2020-04-04"}
                },
                rel.Columns
            );

            rel = FindRelation(rsp.Output, ":price");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"12.50", "14.25", "11.00", "12.25"}
                },
                rel.Columns
            );

            rel = FindRelation(rsp.Output, ":quantity");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"2", "4", "4", "3"}
                },
                rel.Columns
            );

            rel = FindRelation(rsp.Output, ":cocktail");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"martini", "sazerac", "cosmopolitan", "bellini"}
                },
                rel.Columns
            );
        }

        private const string SampleNoHeader = "" +
                                              "\"martini\",2,12.50,\"2020-01-01\"\n" +
                                              "\"sazerac\",4,14.25,\"2020-02-02\"\n" +
                                              "\"cosmopolitan\",4,11.00,\"2020-03-03\"\n" +
                                              "\"bellini\",3,12.25,\"2020-04-04\"\n";

        [Fact]
        public async Task LoadCsvNoHeaderTest()
        {
            var client = CreateClient();

            var createRsp = await CreateEngineWaitAsync(client);
            await client.CreateDatabaseAsync(Dbname, createRsp.Name);

            var opts = new CsvOptions().WithHeaderRow(0);
            var loadRsp = await client.LoadCsvAsync(Dbname, createRsp.Name, "sample_no_header", SampleNoHeader, opts);
            Assert.False(loadRsp.Aborted);
            Assert.Empty(loadRsp.Output);
            Assert.Empty(loadRsp.Problems);

            var rsp = await client.ExecuteV1Async(Dbname, createRsp.Name, "def output = sample_no_header");

            var rel = FindRelation(rsp.Output, ":COL1");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {1L, 2L, 3L, 4L},
                    new object[] {"martini", "sazerac", "cosmopolitan", "bellini"}
                },
                rel.Columns
            );


            rel = FindRelation(rsp.Output, ":COL2");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {1L, 2L, 3L, 4L},
                    new object[] {"2", "4", "4", "3"}
                },
                rel.Columns
            );

            rel = FindRelation(rsp.Output, ":COL3");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {1L, 2L, 3L, 4L},
                    new object[] {"12.50", "14.25", "11.00", "12.25"}
                },
                rel.Columns
            );

            rel = FindRelation(rsp.Output, ":COL4");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {1L, 2L, 3L, 4L},
                    new object[] {"2020-01-01", "2020-02-02", "2020-03-03", "2020-04-04"}
                },
                rel.Columns
            );
        }

        private const string SampleAltSyntax = "" +
                                               "cocktail|quantity|price|date\n" +
                                               "'martini'|2|12.50|'2020-01-01'\n" +
                                               "'sazerac'|4|14.25|'2020-02-02'\n" +
                                               "'cosmopolitan'|4|11.00|'2020-03-03'\n" +
                                               "'bellini'|3|12.25|'2020-04-04'\n";

        [Fact]
        public async Task LoadCsvAltSyntaxTest()
        {
            var client = CreateClient();

            var createRsp = await CreateEngineWaitAsync(client);
            await client.CreateDatabaseAsync(Dbname, createRsp.Name);

            var opts = new CsvOptions().WithDelim('|').WithQuoteChar('\'');
            var loadRsp = await client.LoadCsvAsync(Dbname, createRsp.Name, "sample_alt_syntax", SampleAltSyntax, opts);
            Assert.False(loadRsp.Aborted);
            Assert.Empty(loadRsp.Output);
            Assert.Empty(loadRsp.Problems);

            var rsp = await client.ExecuteV1Async(Dbname, createRsp.Name, "def output = sample_alt_syntax");

            var rel = FindRelation(rsp.Output, ":date");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"2020-01-01", "2020-02-02", "2020-03-03", "2020-04-04"}
                },
                rel.Columns
            );

            rel = FindRelation(rsp.Output, ":price");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"12.50", "14.25", "11.00", "12.25"}
                },
                rel.Columns
            );

            rel = FindRelation(rsp.Output, ":quantity");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"2", "4", "4", "3"}
                },
                rel.Columns
            );

            rel = FindRelation(rsp.Output, ":cocktail");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"martini", "sazerac", "cosmopolitan", "bellini"}
                },
                rel.Columns
            );
        }

        [Fact]
        public async Task LoadCsvWithSchemaTest()
        {
            var client = CreateClient();

            var createRsp = await CreateEngineWaitAsync(client);
            await client.CreateDatabaseAsync(Dbname, createRsp.Name);

            var schema = new Dictionary<string, string>
            {
                { "cocktail", "string" },
                { "quantity", "int" },
                { "price", "decimal(64,2)" },
                { "date", "date" }
            };

            var opts = new CsvOptions().WithSchema(schema);
            var loadRsp = await client.LoadCsvAsync(Dbname, createRsp.Name, "sample", Sample, opts);
            Assert.False(loadRsp.Aborted);
            Assert.Empty(loadRsp.Output);
            Assert.Empty(loadRsp.Problems);

            var rsp = await client.ExecuteV1Async(Dbname, createRsp.Name, "def output = sample");

            var rel = FindRelation(rsp.Output, ":date");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"2020-01-01", "2020-02-02", "2020-03-03", "2020-04-04"}
                },
                rel.Columns
            );
            Assert.Single(rel.RelKey.Values);
            Assert.Equal("Dates.Date", rel.RelKey.Values[0]);

            rel = FindRelation(rsp.Output, ":price");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {12.5, 14.25, 11.00, 12.25}
                },
                rel.Columns
            );
            Assert.Single(rel.RelKey.Values);
            Assert.Equal("FixedPointDecimals.FixedDecimal{Int64, 2}", rel.RelKey.Values[0]);

            rel = FindRelation(rsp.Output, ":quantity");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {2L, 4L, 4L, 3L}
                },
                rel.Columns
            );
            Assert.Single(rel.RelKey.Values);
            Assert.Equal("Int64", rel.RelKey.Values[0]);

            rel = FindRelation(rsp.Output, ":cocktail");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new[]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"martini", "sazerac", "cosmopolitan", "bellini"}
                },
                rel.Columns
            );
            Assert.Single(rel.RelKey.Values);
            Assert.Equal("String", rel.RelKey.Values[0]);
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try
            {
                await client.DeleteDatabaseAsync(Dbname);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }

            try
            {
                await client.DeleteEngineWaitAsync(GetEngineName());
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }
        }
    }
}
