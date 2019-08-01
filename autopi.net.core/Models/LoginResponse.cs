using Newtonsoft.Json;
using System;

namespace autopi.net.core.Models
{

    public partial class LoginResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }

}