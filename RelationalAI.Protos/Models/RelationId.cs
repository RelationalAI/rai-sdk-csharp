namespace RelationalAI.Protos.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class RelationId
    {
        [JsonProperty("arguments")]
        public List<Argument> Arguments { get; set; }
    }
}
