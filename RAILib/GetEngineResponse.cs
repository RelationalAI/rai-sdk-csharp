namespace RAILib
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class GetEngineResponse : Entity
    {
        [JsonProperty("computes", Required = Required.AllowNull)]
        public List<Engine> Engines { get; set; }
    }
}