using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace autopi.net.core.Models
{
    public partial class StorageDataResponse
    {
        [JsonProperty("ts")]
        public DateTimeOffset Ts { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("value")]
        public float Value { get; set; }

        [JsonProperty("altitude")]
        public float Altitude { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }


    }

    public partial class Boundary
    {
        public List<Location> Bounds { get; set; }
        public string Name { get; set; }
    }
}
