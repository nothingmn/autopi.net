using Newtonsoft.Json;
using System;

namespace autopi.net.core.Models
{
    public partial class Addon
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("settings")]
        public Settings Settings { get; set; }
    }

}