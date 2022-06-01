namespace RelationalAI.Protos.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class MetadataInfos
    {
        [JsonProperty("relations")]
        public List<Relation> Relations { get; set; }
    }
}
