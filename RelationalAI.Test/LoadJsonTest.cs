using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RelationalAI.Test
{
    [Collection("RelationalAI.Test")]
    public class LoadJsonTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{Uuid}";
        private const string Sample = "{" +
                                      "\"name\":\"Amira\",\n" +
                                      "\"age\":32,\n" +
                                      "\"height\":null,\n" +
                                      "\"pets\":[\"dog\",\"rabbit\"]}";

        private readonly ITestOutputHelper outputHelper;
        private readonly EngineFixture engineFixture;

        public LoadJsonTests(EngineFixture fixture, ITestOutputHelper output)
        {
            outputHelper = output;
            engineFixture = fixture;
        }

        [Fact]
        public async Task LoadJsontTest()
        {
            var client = CreateClient(outputHelper);

            await engineFixture.CreateEngineWaitAsync();
            await client.CreateDatabaseAsync(Dbname, engineFixture.Engine.Name);

            var loadRsp = await client.LoadJsonAsync(Dbname, engineFixture.Engine.Name, "sample", Sample);
            Assert.False(loadRsp.Aborted);
            Assert.Empty(loadRsp.Output);
            Assert.Empty(loadRsp.Problems);

            var rsp = await client.ExecuteV1Async(Dbname, engineFixture.Engine.Name, "def output = sample");

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
            var client = CreateClient(outputHelper);

            try
            {
                await client.DeleteDatabaseAsync(Dbname);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }
        }
    }
}
