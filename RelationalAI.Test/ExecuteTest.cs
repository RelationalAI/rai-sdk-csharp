using System;
using Xunit;

namespace RelationalAI.Test
{
    public class ExecuteTests : UnitTest
    {
        public static string UUID = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{UUID}";
        public static string EngineName = $"csharp-sdk-{UUID}";

        [Fact]
        public void ExecuteATest()
        {
            Client client = CreateClient();

            client.CreateEngineWait(EngineName);
            client.CreateDatabase(Dbname, EngineName);

            var query = "x, x^2, x^3, x^4 from x in {1; 2; 3; 4; 5}";
            var rsp = client.Execute(Dbname, EngineName, query, true);

            Console.WriteLine(rsp);
            Assert.Equal(rsp.Aborted, false);
            var output = rsp.Output;
            Assert.Equal(output.Length, 1);
            var relation = output[0];
            var relKey = relation.RelKey;
            Assert.Equal(relKey.Name, "output");
            Assert.Equal(relKey.Keys, new string[] {"Int64", "Int64", "Int64"} );
            Assert.Equal(relKey.Values, new string[] {"Int64"} );
            var columns = relation.Columns;
            var expected = new object[][]
            {
                new object[] {1L, 2L, 3L, 4L, 5L},
                new object[] {1L, 4L, 9L, 16L, 25L},
                new object[] {1L, 8L, 27L, 64L, 125L},
                new object[] {1L, 16L, 81L, 256L, 625L}
            };

            Assert.Equal(expected, columns);
        }

        public override void Dispose()
        {
            var client = CreateClient();

            try { client.DeleteDatabase(Dbname); } catch {}
            try { client.DeleteEngineWait(EngineName); } catch {}
        }
    }
}
