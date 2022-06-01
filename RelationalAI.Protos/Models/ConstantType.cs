namespace RelationalAI.Protos.Models
{
    using Newtonsoft.Json;

    public class ConstantType
    {
        [JsonProperty("relType")]
        public RelType RelType { get; set; }

        [JsonProperty("value")]
        public Value Value { get; set; }
    }
}
