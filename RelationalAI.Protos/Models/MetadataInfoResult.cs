using System.Collections.Generic;
using Newtonsoft.Json;

namespace RelationalAI.Protos.Models
{
    public class MetadataInfoResult
    {
       [JsonProperty("relations")]
       public List<Relation> Relations { get; set; }

    }
}
