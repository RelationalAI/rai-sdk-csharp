using System;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class ExecuteTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{Uuid}";

        [Fact]
        public async Task ExecuteV1Test()
        {
            var client = CreateClient();

            var engineName = EngineHelper.Instance.EngineName; 
            await client.CreateDatabaseAsync(Dbname, engineName);

            var query = "x, x^2, x^3, x^4 from x in {1; 2; 3; 4; 5}";
            var rsp = await client.ExecuteV1Async(Dbname, engineName, query, true);

            Assert.False(rsp.Aborted);
            var output = rsp.Output;
            Assert.Single(output);
            var relation = output[0];
            var relKey = relation.RelKey;
            Assert.Equal("output", relKey.Name);
            Assert.Equal(relKey.Keys, new[] { "Int64", "Int64", "Int64" });
            Assert.Equal(relKey.Values, new[] { "Int64" });
            var columns = relation.Columns;
            var expected = new[]
            {
                new object[] {1L, 2L, 3L, 4L, 5L},
                new object[] {1L, 4L, 9L, 16L, 25L},
                new object[] {1L, 8L, 27L, 64L, 125L},
                new object[] {1L, 16L, 81L, 256L, 625L}
            };

            Assert.Equal(expected, columns);
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
                EngineHelper.Instance.DeleteEngineAsync();
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }
        }

        public override async Task InitializeAsync()
        {
             await EngineHelper.Instance.CreateOrGetEngineAsync();
        }
    }
}
