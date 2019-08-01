using Newtonsoft.Json;
using System;

namespace autopi.net.core.Models
{
    public partial class Dashboard
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("default")]
        public bool Default { get; set; }
    }

}