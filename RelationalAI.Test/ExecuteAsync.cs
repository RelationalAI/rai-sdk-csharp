using System;
using System.Collections.Generic;
using System.IO;
using Relationalai.Protocol;
using Xunit;
using System.Threading.Tasks;

namespace RelationalAI.Test
{
    public class ExecuteAsyncTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{Uuid}";
        
        [Fact]
        public async Task ExecuteAsyncTest()
        {
            var client = CreateClient();

            var engineName = EngineHelper.Instance.EngineName; 
            await client.CreateDatabaseAsync(Dbname, engineName);

            var query = "x, x^2, x^3, x^4 from x in {1; 2; 3; 4; 5}";
            var rsp = await client.ExecuteWaitAsync(Dbname, engineName, query, true);

            var results = new List<ArrowRelation>
            {
                new ArrowRelation("/:output/Int64/Int64/Int64/Int64", new List<object> {1L, 2L, 3L, 4L, 5L} ),
                new ArrowRelation("/:output/Int64/Int64/Int64/Int64", new List<object> {1L, 4L, 9L, 16L, 25L} ),
                new ArrowRelation("/:output/Int64/Int64/Int64/Int64", new List<object> {1L, 8L, 27L, 64L, 125L} ),
                new ArrowRelation("/:output/Int64/Int64/Int64/Int64", new List<object> {1L, 16L, 81L, 256L, 625L} )
            };

            var metadata = MetadataInfo.Parser.ParseFrom(File.ReadAllBytes("../../../metadata.pb"));

            var problems = new List<object>();

            Assert.Equal(results, rsp.Results);
            Assert.Equal(metadata.ToString(), rsp.Metadata.ToString());
            Assert.Equal(problems, rsp.Problems);
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
