using System;
using System.Collections.Generic;
using Xunit;

namespace RelationalAI.Test
{
    public class LoadCsvTests : UnitTest
    {
        public static string UUID = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{UUID}";
        public static string EngineName = $"csharp-sdk-{UUID}";
        string sample = "" +
            "cocktail,quantity,price,date\n" +
            "\"martini\",2,12.50,\"2020-01-01\"\n" +
            "\"sazerac\",4,14.25,\"2020-02-02\"\n" +
            "\"cosmopolitan\",4,11.00,\"2020-03-03\"\n" +
            "\"bellini\",3,12.25,\"2020-04-04\"\n";

        [Fact]
        public void LoadCsvtTest()
        {
            Client client = CreateClient();

            client.CreateEngineWait(EngineName);
            client.CreateDatabase(Dbname, EngineName);

            var loadRsp = client.LoadCsv(Dbname, EngineName, "sample", sample);
            Assert.Equal(false, loadRsp.Aborted);
            Assert.Equal(0, loadRsp.Output.Length);
            Assert.Equal(0, loadRsp.Problems.Length);

            var rsp = client.ExecuteV1(Dbname, EngineName, "def output = sample");

            Relation rel;

            rel = findRelation(rsp.Output, ":date");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"2020-01-01", "2020-02-02", "2020-03-03", "2020-04-04"}
                },
                rel.Columns
            );

            rel = findRelation(rsp.Output, ":price");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"12.50", "14.25", "11.00", "12.25"}
                },
                rel.Columns
            );

            rel = findRelation(rsp.Output, ":quantity");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][] {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"2", "4", "4", "3"}
                },
                rel.Columns
            );

            rel = findRelation(rsp.Output, ":cocktail");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"martini", "sazerac", "cosmopolitan", "bellini"}
                },
                rel.Columns
            );
        }

        string sampleNoHeader = "" +
            "\"martini\",2,12.50,\"2020-01-01\"\n" +
            "\"sazerac\",4,14.25,\"2020-02-02\"\n" +
            "\"cosmopolitan\",4,11.00,\"2020-03-03\"\n" +
            "\"bellini\",3,12.25,\"2020-04-04\"\n";

        [Fact]
        public void LoadCsvNoHeaderTest()
        {
            Client client = CreateClient();

            client.CreateEngineWait(EngineName);
            client.CreateDatabase(Dbname, EngineName);

            var opts = new CsvOptions().WithHeaderRow(0);
            var loadRsp = client.LoadCsv(Dbname, EngineName, "sample_no_header", sampleNoHeader, opts);
            Assert.Equal(false, loadRsp.Aborted);
            Assert.Equal(0, loadRsp.Output.Length);
            Assert.Equal(0, loadRsp.Problems.Length);

            var rsp = client.ExecuteV1(Dbname, EngineName, "def output = sample_no_header");

            Relation rel;

            rel = findRelation(rsp.Output, ":COL1");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {1L, 2L, 3L, 4L},
                    new object[] {"martini", "sazerac", "cosmopolitan", "bellini"}
                },
                rel.Columns
            );


            rel = findRelation(rsp.Output, ":COL2");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {1L, 2L, 3L, 4L},
                    new object[] {"2", "4", "4", "3"}
                },
                rel.Columns
            );

            rel = findRelation(rsp.Output, ":COL3");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {1L, 2L, 3L, 4L},
                    new object[] {"12.50", "14.25", "11.00", "12.25"}
                },
                rel.Columns
            );

            rel = findRelation(rsp.Output, ":COL4");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {1L, 2L, 3L, 4L},
                    new object[] {"2020-01-01", "2020-02-02", "2020-03-03", "2020-04-04"}
                },
                rel.Columns
            );
        }

        string sampleAltSyntax = "" +
            "cocktail|quantity|price|date\n" +
            "'martini'|2|12.50|'2020-01-01'\n" +
            "'sazerac'|4|14.25|'2020-02-02'\n" +
            "'cosmopolitan'|4|11.00|'2020-03-03'\n" +
            "'bellini'|3|12.25|'2020-04-04'\n";

        [Fact]
        public void LoadCsvAltSyntaxTest()
        {
            Client client = CreateClient();

            client.CreateEngineWait(EngineName);
            client.CreateDatabase(Dbname, EngineName);

            var opts = new CsvOptions().WithDelim('|').WithQuoteChar('\'');
            var loadRsp = client.LoadCsv(Dbname, EngineName, "sample_alt_syntax", sampleAltSyntax, opts);
            Assert.Equal(false, loadRsp.Aborted);
            Assert.Equal(0, loadRsp.Output.Length);
            Assert.Equal(0, loadRsp.Problems.Length);

            var rsp = client.ExecuteV1(Dbname, EngineName, "def output = sample_alt_syntax");

            Relation rel;

            rel = findRelation(rsp.Output, ":date");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"2020-01-01", "2020-02-02", "2020-03-03", "2020-04-04"}
                },
                rel.Columns
            );

            rel = findRelation(rsp.Output, ":price");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"12.50", "14.25", "11.00", "12.25"}
                },
                rel.Columns
            );

            rel = findRelation(rsp.Output, ":quantity");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"2", "4", "4", "3"}
                },
                rel.Columns
            );

            rel = findRelation(rsp.Output, ":cocktail");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"martini", "sazerac", "cosmopolitan", "bellini"}
                },
                rel.Columns
            );
        }

        [Fact]
        public void LoadCsvWithSchemaTest()
        {
            Client client = CreateClient();

            client.CreateEngineWait(EngineName);
            client.CreateDatabase(Dbname, EngineName);

            var schema = new Dictionary<string, string>();
            schema.Add("cocktail", "string");
            schema.Add("quantity", "int");
            schema.Add("price", "decimal(64,2)");
            schema.Add("date", "date");

            var opts = new CsvOptions().WithSchema(schema);
            var loadRsp = client.LoadCsv(Dbname, EngineName, "sample", sample, opts);
            Assert.Equal(false, loadRsp.Aborted);
            Assert.Equal(0, loadRsp.Output.Length);
            Assert.Equal(0, loadRsp.Problems.Length);

            var rsp = client.ExecuteV1(Dbname, EngineName, "def output = sample");

            Relation rel;

            rel = findRelation(rsp.Output, ":date");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"2020-01-01", "2020-02-02", "2020-03-03", "2020-04-04"}
                },
                rel.Columns
            );
            Assert.Equal(1, rel.RelKey.Values.Length);
            Assert.Equal("Dates.Date", rel.RelKey.Values[0]);

            rel = findRelation(rsp.Output, ":price");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {12.5, 14.25, 11.00, 12.25}
                },
                rel.Columns
            );
            Assert.Equal(1, rel.RelKey.Values.Length);
            Assert.Equal("FixedPointDecimals.FixedDecimal{Int64, 2}", rel.RelKey.Values[0]);

            rel = findRelation(rsp.Output, ":quantity");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {2L, 4L, 4L, 3L}
                },
                rel.Columns
            );
            Assert.Equal(1, rel.RelKey.Values.Length);
            Assert.Equal("Int64", rel.RelKey.Values[0]);

            rel = findRelation(rsp.Output, ":cocktail");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(
                new object[][]
                {
                    new object[] {2L, 3L, 4L, 5L},
                    new object[] {"martini", "sazerac", "cosmopolitan", "bellini"}
                },
                rel.Columns
            );
            Assert.Equal(1, rel.RelKey.Values.Length);
            Assert.Equal("String", rel.RelKey.Values[0]);
        }

        public override void Dispose()
        {
            var client = CreateClient();

            try { client.DeleteDatabase(Dbname); } catch {}
            try { client.DeleteEngineWait(EngineName); } catch {}
        }
    }
}
