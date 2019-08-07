using System;
using Newtonsoft.Json;

namespace autopi.net.core.Models
{
    public partial class StorageDataResponse
    {
        [JsonProperty("ts")]
        public DateTimeOffset Ts { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }
    }
}
