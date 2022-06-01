using System.Collections.Generic;
using Newtonsoft.Json;

namespace RelationalAI.Protos.Models
{
    public class Value
    {
        [JsonProperty("arguments")]
        public List<Argument> Arguments { get; set; }
    }
}
