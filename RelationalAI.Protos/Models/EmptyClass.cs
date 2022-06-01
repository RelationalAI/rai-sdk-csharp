using Newtonsoft.Json;

namespace RelationalAI.Protos.Models
{
    public class EmptyClass
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("constantType")]
        public ConstantType ConstantType { get; set; }

        [JsonProperty("primitiveType")]
        public string PrimitiveType { get; set; }

        [JsonProperty("stringVal")]
        public string StringVal { get; set; }
    }
}
