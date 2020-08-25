﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rexobot.Gumroad.Responses
{
    public class GetProductsResponse
    {
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }
        [JsonProperty("products")]
        public IReadOnlyList<GumroadProduct> Products { get; set; }
    }
}
