namespace RelationalAI.Protos.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Value
    {
        [JsonProperty("arguments")]
        public List<Argument> Arguments { get; set; }
    }
}
