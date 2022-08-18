using System;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class LoadJsonTests : UnitTest
    {
        public static string UUID = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{UUID}";
        public static string EngineName = $"csharp-sdk-{UUID}";
        string sample = "{" +
            "\"name\":\"Amira\",\n" +
            "\"age\":32,\n" +
            "\"height\":null,\n" +
            "\"pets\":[\"dog\",\"rabbit\"]}";

        [Fact]
        public async Task LoadJsontTest()
        {
            Client client = CreateClient();

            await client.CreateEngineWaitAsync(EngineName);
            await client.CreateDatabaseAsync(Dbname, EngineName);

            var loadRsp = await client.LoadJsonAsync(Dbname, EngineName, "sample", sample);
            Assert.Equal(false, loadRsp.Aborted);
            Assert.Equal(0, loadRsp.Output.Length);
            Assert.Equal(0, loadRsp.Problems.Length);

            var rsp = await client.ExecuteV1Async(Dbname, EngineName, "def output = sample");

            Relation rel;

            rel = findRelation(rsp.Output, ":name");
            Assert.NotNull(rel);
            Assert.Equal(1, rel.Columns.Length);
            Assert.Equal(new[] { new object[] {"Amira"} }, rel.Columns);

            rel = findRelation(rsp.Output, ":age");
            Assert.NotNull(rel);
            Assert.Equal(1, rel.Columns.Length);
            Assert.Equal(new[] { new object[] { 32L } }, rel.Columns);

            rel = findRelation(rsp.Output, ":height");
            Assert.NotNull(rel);
            Assert.Equal(1, rel.Columns.Length);
            Assert.Equal(new[] { new object [] { null } }, rel.Columns);

            rel = findRelation(rsp.Output, ":pets");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(new[] { new object [] { 1L, 2L }, new object [] { "dog", "rabbit" } }, rel.Columns);
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try { await client.DeleteDatabaseAsync(Dbname); } catch {}
            try { await client.DeleteEngineWaitAsync(EngineName); } catch {}
        }
    }
}
