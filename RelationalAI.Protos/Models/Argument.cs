namespace RelationalAI.Protos.Models
{
    using Newtonsoft.Json;

    public class Argument
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("constantType", NullValueHandling = NullValueHandling.Ignore)]
        public ConstantType ConstantType { get; set; }

        [JsonProperty("primitiveType", NullValueHandling = NullValueHandling.Ignore)]
        public string PrimitiveType { get; set; }

        [JsonProperty("stringVal", NullValueHandling = NullValueHandling.Ignore)]
        public string StringVal { get; set; }
    }
}
