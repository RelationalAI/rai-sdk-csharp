using System;
using Xunit;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json;
using System.Web;
using System.Linq;
using RAILib;
namespace RAILib.Test
{
    public class RAILibUnitTests
    {
        [Fact]
        public void Test1()
        {
            Dictionary<string, object> config = Config.Read();
            Api.Context context = new Api.Context(config);
            Api api = new Api(context);
            Console.WriteLine(api.ListDatabases());
            
       }
    }
}
