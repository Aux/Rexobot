using Newtonsoft.Json;

namespace Rexobot.Gumroad
{
    public class GetUserResponse
    {
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }
        [JsonProperty("user")]
        public GumroadUser User { get; set; }
    }
}
