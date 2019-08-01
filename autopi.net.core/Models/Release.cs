using System;
using Newtonsoft.Json;

namespace autopi.net.core.Models
{
    public partial class Release
    {
        [JsonProperty("version")]
        public string Version { get; set; }
    }

}