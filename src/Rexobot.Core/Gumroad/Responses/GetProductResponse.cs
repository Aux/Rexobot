using Newtonsoft.Json;

namespace Rexobot.Gumroad
{
    public class GetProductResponse
    {
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }
        [JsonProperty("product")]
        public GumroadProduct Product { get; set; }
    }
}
