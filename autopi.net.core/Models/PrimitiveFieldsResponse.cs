using Newtonsoft.Json;
using System;

namespace autopi.net.core.Models
{
    public partial class PrimitiveFieldsResponse
    {
        public string Field { get; set; }

        [JsonProperty("max_ts")]
        public double MaxTs { get; set; }

        [JsonProperty("ts")]
        public DateTimeOffset Ts { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }
}
