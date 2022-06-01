using Newtonsoft.Json;

namespace RelationalAI.Protos.Models
{
    public class Relation
    {
        [JsonProperty("relationId")]
        public RelationId RelationId { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }
    }
}
