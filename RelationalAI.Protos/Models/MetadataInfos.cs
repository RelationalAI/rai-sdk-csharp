using System.Collections.Generic;
using Newtonsoft.Json;

namespace RelationalAI.Protos.Models
{
    public class MetadataInfos
    {
       [JsonProperty("relations")]
       public List<Relation> Relations { get; set; }

    }
}
