namespace RAILib
{
    using Newtonsoft.Json;

    public class CreateEngineResponse : Entity
    {
        [JsonProperty("compute", Required = Required.Always)]
        public Engine Engine { get; set; }
    }
}