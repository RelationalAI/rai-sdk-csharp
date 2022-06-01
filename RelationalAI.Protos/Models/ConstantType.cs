using Newtonsoft.Json;

namespace RelationalAI.Protos.Models
{
    public class ConstantType
    {
        [JsonProperty("relType")]
        public RelType RelType { get; set; }

        [JsonProperty("value")]
        public Value Value { get; set; }
    }
}
