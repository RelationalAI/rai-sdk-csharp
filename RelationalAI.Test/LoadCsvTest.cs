using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class LoadCsvTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{Uuid}";
        public static string EngineName = $"csharp-sdk-{Uuid}";

        private const string Sample = "" +
                                      "cocktail,quantity,price,date\n" +
                                      "\"martini\",2,12.50,\"2020-01-01\"\n" +
                                      "\"sazerac\",4,14.25,\"2020-02-02\"\n" +
                                      "\"cosmopolitan\",4,11.00,\"2020-03-03\"\n" +
                                      "\"bellini\",3,12.25,\"2020-04-04\"\n";

        [Fact]
        public async Task LoadCsvTest()
        {
            var client = CreateClient();

            await client.CreateEngineWaitAsync(EngineName);
            await client.CreateDatabaseAsync(Dbname, EngineName);

            var loadRsp = await client.LoadCsvAsync(Dbname, EngineName, "sample", Sample);
            Assert.Equal(TransactionAsyncState.Completed, loadRsp.Transaction.State);
            Assert.Empty(loadRsp.Problems);

            var rsp = await client.ExecuteAsync(Dbname, EngineName, "def output = sample");
            Assert.Equal(rsp.Results[0].Table, new List<object> { 2L, 3L, 4L, 5L });
            Assert.Equal(rsp.Results[1].Table, new List<object> { "martini", "sazerac", "cosmopolitan", "bellini" });
            Assert.Equal(rsp.Results[2].Table, new List<object> { 2L, 3L, 4L, 5L });
            Assert.Equal(rsp.Results[3].Table, new List<object> { "2020-01-01", "2020-02-02", "2020-03-03", "2020-04-04" });
            Assert.Equal(rsp.Results[4].Table, new List<object> { 2L, 3L, 4L, 5L });
            Assert.Equal(rsp.Results[5].Table, new List<object> { "12.50", "14.25", "11.00", "12.25" });
            Assert.Equal(rsp.Results[6].Table, new List<object> { 2L, 3L, 4L, 5L });
            Assert.Equal(rsp.Results[7].Table, new List<object> { "2", "4", "4", "3" });
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

            await client.CreateEngineWaitAsync(EngineName);
            await client.CreateDatabaseAsync(Dbname, EngineName);

            var opts = new CsvOptions().WithHeaderRow(0);
            var loadRsp = await client.LoadCsvAsync(Dbname, EngineName, "sample_no_header", SampleNoHeader, opts);
            Assert.Equal(TransactionAsyncState.Completed, loadRsp.Transaction.State);
            Assert.Empty(loadRsp.Problems);

            var rsp = await client.ExecuteAsync(Dbname, EngineName, "def output = sample_no_header");
            Assert.Equal(rsp.Results[0].Table, new List<object> { 1L, 2L, 3L, 4L });
            Assert.Equal(rsp.Results[1].Table, new List<object> { "martini", "sazerac", "cosmopolitan", "bellini" });
            Assert.Equal(rsp.Results[2].Table, new List<object> { 1L, 2L, 3L, 4L });
            Assert.Equal(rsp.Results[3].Table, new List<object> { "2", "4", "4", "3" });
            Assert.Equal(rsp.Results[4].Table, new List<object> { 1L, 2L, 3L, 4L });
            Assert.Equal(rsp.Results[5].Table, new List<object> { "12.50", "14.25", "11.00", "12.25" });
            Assert.Equal(rsp.Results[6].Table, new List<object> { 1L, 2L, 3L, 4L });
            Assert.Equal(rsp.Results[7].Table, new List<object> { "2020-01-01", "2020-02-02", "2020-03-03", "2020-04-04" });
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

            await client.CreateEngineWaitAsync(EngineName);
            await client.CreateDatabaseAsync(Dbname, EngineName);

            var opts = new CsvOptions().WithDelim('|').WithQuoteChar('\'');
            var loadRsp = await client.LoadCsvAsync(Dbname, EngineName, "sample_alt_syntax", SampleAltSyntax, opts);
            Assert.Equal(TransactionAsyncState.Completed, loadRsp.Transaction.State);
            Assert.Empty(loadRsp.Problems);

            var rsp = await client.ExecuteAsync(Dbname, EngineName, "def output = sample_alt_syntax");
            Assert.Equal(rsp.Results[0].Table, new List<object> { 2L, 3L, 4L, 5L });
            Assert.Equal(rsp.Results[1].Table, new List<object> { "martini", "sazerac", "cosmopolitan", "bellini" });
            Assert.Equal(rsp.Results[2].Table, new List<object> { 2L, 3L, 4L, 5L });
            Assert.Equal(rsp.Results[3].Table, new List<object> { "2020-01-01", "2020-02-02", "2020-03-03", "2020-04-04" });
            Assert.Equal(rsp.Results[4].Table, new List<object> { 2L, 3L, 4L, 5L });
            Assert.Equal(rsp.Results[5].Table, new List<object> { "12.50", "14.25", "11.00", "12.25" });
            Assert.Equal(rsp.Results[6].Table, new List<object> { 2L, 3L, 4L, 5L });
            Assert.Equal(rsp.Results[7].Table, new List<object> { "2", "4", "4", "3" });
        }

        [Fact]
        public async Task LoadCsvWithSchemaTest()
        {
            var client = CreateClient();

            await client.CreateEngineWaitAsync(EngineName);
            await client.CreateDatabaseAsync(Dbname, EngineName);

            var schema = new Dictionary<string, string>
            {
                { "cocktail", "string" },
                { "quantity", "int" },
                { "price", "decimal(64,2)" },
                { "date", "date" }
            };

            var opts = new CsvOptions().WithSchema(schema);
            var loadRsp = await client.LoadCsvAsync(Dbname, EngineName, "sample", Sample, opts);
            Assert.Equal(TransactionAsyncState.Completed, loadRsp.Transaction.State);
            Assert.Empty(loadRsp.Problems);

            var rsp = await client.ExecuteAsync(Dbname, EngineName, "def output = sample");
            Assert.Equal(rsp.Results[0].Table, new List<object> { 2L, 3L, 4L, 5L });
            Assert.Equal(rsp.Results[1].Table, new List<object> { "martini", "sazerac", "cosmopolitan", "bellini" });
            Assert.Equal(rsp.Results[2].Table, new List<object> { 2L, 3L, 4L, 5L });
            Assert.Equal(rsp.Results[3].Table, new List<object> { 737425L, 737457L, 737487L, 737519L });
            Assert.Equal(rsp.Results[4].Table, new List<object> { 2L, 3L, 4L, 5L });
            Assert.Equal(rsp.Results[5].Table, new List<object> { 1250L, 1425L, 1100L, 1225L });
            Assert.Equal(rsp.Results[6].Table, new List<object> { 2L, 3L, 4L, 5L });
            Assert.Equal(rsp.Results[7].Table, new List<object> { 2L, 4L, 4L, 3L });
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
                await client.DeleteEngineWaitAsync(EngineName);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }
        }
    }
}
