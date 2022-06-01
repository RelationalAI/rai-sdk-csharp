namespace RelationalAI.Protos.Models
{
    using Newtonsoft.Json;

    public class RelType
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("primitiveType")]
        public string PrimitiveType { get; set; }
    }
}
