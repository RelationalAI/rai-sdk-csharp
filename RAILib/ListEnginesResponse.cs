namespace RAILib
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ListEnginesResponse : Entity
    {
        [JsonProperty("computes", Required = Required.Always)]
        public List<Engine> Engines { get; set; }
    }
}