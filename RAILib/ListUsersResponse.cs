namespace RAILib
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ListUsersResponse : Entity
    {
        [JsonProperty("users", Required = Required.Always)]
        public List<User> Users { get; set; }
    }
}