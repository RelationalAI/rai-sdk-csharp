namespace RAILib
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ListOAuthClientResponse : Entity
    {
        [JsonProperty("clients", Required = Required.Always)]
        public List<OAuthClient> Clients { get; set; }
    }
}