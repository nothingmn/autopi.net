using System;
using Newtonsoft.Json;

namespace autopi.net.core.Models
{

    public partial class Vehicle
    {

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("vin")]
        public string Vin { get; set; }

        [JsonProperty("callName")]
        public string CallName { get; set; }

        [JsonProperty("licensePlate")]
        public string LicensePlate { get; set; }

        [JsonProperty("model")]
        public long Model { get; set; }

        [JsonProperty("make")]
        public long Make { get; set; }

        [JsonProperty("year")]
        public long Year { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

}