using System;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class LoadJsonTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{Uuid}";
        public static string EngineName = $"csharp-sdk-{Uuid}";

        private const string Sample = "{" +
                                      "\"name\":\"Amira\",\n" +
                                      "\"age\":32,\n" +
                                      "\"height\":null,\n" +
                                      "\"pets\":[\"dog\",\"rabbit\"]}";

        [Fact]
        public async Task LoadJsonTest()
        {
            var client = CreateClient();

            await client.CreateEngineWaitAsync(EngineName);
            await client.CreateDatabaseAsync(Dbname, EngineName);

            var loadRsp = await client.LoadJsonAsync(Dbname, EngineName, "sample", Sample);
            Assert.Equal(TransactionAsyncState.Completed, loadRsp.Transaction.State);
            Assert.Empty(loadRsp.Problems);

            // FIXME: complete the test implementation
            // when this issue https://github.com/RelationalAI/rai-sdk-csharp/issues/25
            // is fixed
            //var rsp = await client.ExecuteAsync(Dbname, EngineName, "def output = sample");
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
