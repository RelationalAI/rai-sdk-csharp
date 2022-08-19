// <copyright file="ListEnginesResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace RelationalAI
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ListEnginesResponse : Entity
    {
        [JsonProperty("computes", Required = Required.Always)]
        public List<Engine> Engines { get; set; }
    }
}