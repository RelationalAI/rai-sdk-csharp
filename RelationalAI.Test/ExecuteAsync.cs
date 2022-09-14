using System;
using System.Collections.Generic;
using System.IO;
using Relationalai.Protocol;
using Xunit;
using System.Threading.Tasks;
using RelationalAI.Models.Transaction;
using Apache.Arrow;
using Apache.Arrow.Types;

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

            // mock arrow table
            Schema.Builder builder = new Schema.Builder();
            builder.Field(new Field("v1", Int64Type.Default, false));
            builder.Field(new Field("v2", Int64Type.Default, false));
            builder.Field(new Field("v3", Int64Type.Default, false));
            builder.Field(new Field("v4", Int64Type.Default, false));

            var recordBatch = new RecordBatch
            (
                builder.Build(),
                new List<IArrowArray>
                {
                    new Int64Array.Builder().AppendRange(new List<long> { 1, 2, 3, 4, 5 }).Build(),
                    new Int64Array.Builder().AppendRange(new List<long> { 1, 4, 9, 16, 25 }).Build(),
                    new Int64Array.Builder().AppendRange(new List<long> { 1, 8, 27, 64, 125 }).Build(),
                    new Int64Array.Builder().AppendRange(new List<long> { 1, 16, 81, 256, 625 }).Build()
                },
                4
            );

            var table = Table.TableFromRecordBatches(recordBatch.Schema, new List<RecordBatch> { recordBatch });

            Assert.Equal(table, rsp.Results[0].Table);
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
