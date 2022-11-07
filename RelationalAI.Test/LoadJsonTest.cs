using System;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class LoadJsonTests : UnitTest
    {
        public static string Dbname = $"csharp-sdk-{Uuid}";

        private const string Sample = "{" +
                                      "\"name\":\"Amira\",\n" +
                                      "\"age\":32,\n" +
                                      "\"height\":null,\n" +
                                      "\"pets\":[\"dog\",\"rabbit\"]}";

        [Fact]
        public async Task LoadJsontTest()
        {
            var client = CreateClient();

            var createRsp = await CreateEngineWaitAsync(client);
            await client.CreateDatabaseAsync(Dbname, createRsp.Name);

            var loadRsp = await client.LoadJsonAsync(Dbname, createRsp.Name, "sample", Sample);
            Assert.False(loadRsp.Aborted);
            Assert.Empty(loadRsp.Output);
            Assert.Empty(loadRsp.Problems);

            var rsp = await client.ExecuteV1Async(Dbname, createRsp.Name, "def output = sample");

            var rel = FindRelation(rsp.Output, ":name");
            Assert.NotNull(rel);
            Assert.Single(rel.Columns);
            Assert.Equal(new[] { new object[] { "Amira" } }, rel.Columns);

            rel = FindRelation(rsp.Output, ":age");
            Assert.NotNull(rel);
            Assert.Single(rel.Columns);
            Assert.Equal(new[] { new object[] { 32L } }, rel.Columns);

            rel = FindRelation(rsp.Output, ":height");
            Assert.NotNull(rel);
            Assert.Single(rel.Columns);
            Assert.Equal(new[] { new object[] { null } }, rel.Columns);

            rel = FindRelation(rsp.Output, ":pets");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(new[] { new object[] { 1L, 2L }, new object[] { "dog", "rabbit" } }, rel.Columns);
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
