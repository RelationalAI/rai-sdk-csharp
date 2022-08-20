using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class ExecuteAsyncTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{Uuid}";
        public static string EngineName = $"csharp-sdk-{Uuid}";
        [Fact]
        public async Task ExecuteAsyncTest()
        {
            var client = CreateClient();

            await client.CreateEngineWaitAsync(EngineName);
            await client.CreateDatabaseAsync(Dbname, EngineName);

            var query = "x, x^2, x^3, x^4 from x in {1; 2; 3; 4; 5}";
            var rsp = await client.ExecuteWaitAsync(Dbname, EngineName, query, true);

            var results = new List<ArrowRelation>
            {
                new ArrowRelation("v1", new List<object> {1L, 2L, 3L, 4L, 5L} ),
                new ArrowRelation("v2", new List<object> {1L, 4L, 9L, 16L, 25L} ),
                new ArrowRelation("v3", new List<object> {1L, 8L, 27L, 64L, 125L} ),
                new ArrowRelation("v4", new List<object> {1L, 16L, 81L, 256L, 625L} )
            };

            var metadata = new List<TransactionAsyncMetadataResponse>
            {
                new TransactionAsyncMetadataResponse("/:output/Int64/Int64/Int64/Int64", new List<string> {":output", "Int64", "Int64", "Int64", "Int64"})
            };

            var problems = new List<object>();

            Assert.Equal(rsp.Results, results);
            Assert.Equal(rsp.Metadata, metadata);
            Assert.Equal(rsp.Problems, problems);
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try { await client.DeleteDatabaseAsync(Dbname); } catch { }
            try { await client.DeleteEngineWaitAsync(EngineName); } catch { }
        }
    }
}
