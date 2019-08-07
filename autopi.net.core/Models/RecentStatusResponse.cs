using Newtonsoft.Json;
using System;

namespace autopi.net.core.Models
{
    public partial class RecentStatusResponse
    {
        [JsonProperty("latest_com")]
        public DateTimeOffset LatestCom { get; set; }

        [JsonProperty("voltage_level")]
        public long VoltageLevel { get; set; }

        [JsonProperty("voltage")]
        public double Voltage { get; set; }

        [JsonProperty("voltage_ts")]
        public DateTimeOffset VoltageTs { get; set; }

        [JsonProperty("voltage_level_ts")]
        public DateTimeOffset VoltageLevelTs { get; set; }
    }

}
