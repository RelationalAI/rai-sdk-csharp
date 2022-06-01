using Newtonsoft.Json;

namespace RelationalAI.Protos.Models
{
    public class RelType
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("primitiveType")]
        public string PrimitiveType { get; set; }
    }
}
