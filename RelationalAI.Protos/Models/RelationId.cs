using System.Collections.Generic;
using Newtonsoft.Json;

namespace RelationalAI.Protos.Models
{
    public class RelationId
    {
        [JsonProperty("arguments")]
        public List<Argument> Arguments { get; set; }
    }
}
