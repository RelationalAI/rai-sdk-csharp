namespace RelationalAI.Protos.Models
{
    using Newtonsoft.Json;

    public class Relation
    {
        [JsonProperty("relationId")]
        public RelationId RelationId { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }
    }
}
