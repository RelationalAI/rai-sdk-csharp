using System;
using Xunit;

namespace RelationalAI.Test
{
    public class LoadJsonTests : UnitTest
    {
        string sample = "{" +
            "\"name\":\"Amira\",\n" +
            "\"age\":32,\n" +
            "\"height\":null,\n" +
            "\"pets\":[\"dog\",\"rabbit\"]}";

        [Fact]
        public void LoadJsontTest()
        {
            Client client = CreateClient();

            client.CreateEngineWait(EngineName);
            client.CreateDatabase(Dbname, EngineName);

            var loadRsp = client.LoadJson(Dbname, EngineName, "sample", sample);
            Assert.Equal(false, loadRsp.Aborted);
            Assert.Equal(0, loadRsp.Output.Length);
            Assert.Equal(0, loadRsp.Problems.Length);

            var rsp = client.Execute(Dbname, EngineName, "def output = sample");

            Relation rel;

            rel = findRelation(rsp.Output, ":name");
            Assert.NotNull(rel);
            Assert.Equal(1, rel.Columns.Length);
            Assert.Equal(new object [][] { new object[] {"Amira"} }, rel.Columns);

            rel = findRelation(rsp.Output, ":age");
            Assert.NotNull(rel);
            Assert.Equal(1, rel.Columns.Length);
            Assert.Equal(new object [][] { new object[] { 32L } }, rel.Columns);

            rel = findRelation(rsp.Output, ":height");
            Assert.NotNull(rel);
            Assert.Equal(1, rel.Columns.Length);
            Assert.Equal(new object [][] { new object [] { null } }, rel.Columns);

            rel = findRelation(rsp.Output, ":pets");
            Assert.NotNull(rel);
            Assert.Equal(2, rel.Columns.Length);
            Assert.Equal(new object [][] { new object [] { 1L, 2L }, new object [] { "dog", "rabbit" } }, rel.Columns);
        }
    }
}
