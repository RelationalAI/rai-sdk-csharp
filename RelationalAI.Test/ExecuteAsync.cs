using Xunit;
using Newtonsoft.Json.Linq;

namespace RelationalAI.Test
{
    public class ExecuteAsyncTests : UnitTest
    {
        [Fact]
        public void ExecuteAsyncTest()
        {
            Client client = CreateClient();

            client.CreateEngineWait(EngineName);
            client.CreateDatabase(Dbname, EngineName);

            var query = "x, x^2, x^3, x^4 from x in {1; 2; 3; 4; 5}";
            var rsp = client.ExecuteAsyncWait(Dbname, EngineName, query, true);

            JObject expected = new JObject()
            {
                {
                    "results",
                    new JArray()
                    {
                        new JObject()
                        {
                            {"v1", new JArray() {1, 2, 3, 4, 5} },
                            {"v2", new JArray() {1, 4, 9, 16, 25} },
                            {"v3", new JArray() {1, 8, 27, 64, 125} },
                            {"v4", new JArray() {1, 16, 81, 256, 625} }
                        }
                    }
                },
                {
                    "metadata",
                    new JArray()
                    {
                        new JObject()
                        {
                            {"relationId", "/:output/Int64/Int64/Int64/Int64"},
                            {"types", new JArray() {":output", "Int64", "Int64", "Int64", "Int64"} }
                        }
                    }
                },
                {
                    "problems",
                    new JArray()
                }
            };

            Assert.Equal(rsp, expected);
        }
    }
}
