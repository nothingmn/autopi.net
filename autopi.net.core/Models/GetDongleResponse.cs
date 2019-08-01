using System;
using Newtonsoft.Json;

namespace autopi.net.core.Models
{

    public partial class GetDongleResponse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("unit_id")]
        public Guid UnitId { get; set; }

        [JsonProperty("token")]
        public Guid Token { get; set; }

        [JsonProperty("callName")]
        public string CallName { get; set; }

        [JsonProperty("owner")]
        public long Owner { get; set; }

        [JsonProperty("vehicle")]
        public Vehicle Vehicle { get; set; }

        [JsonProperty("display")]
        public string Display { get; set; }

        [JsonProperty("last_communication")]
        public DateTimeOffset LastCommunication { get; set; }

        [JsonProperty("is_updated")]
        public bool IsUpdated { get; set; }

        [JsonProperty("release")]
        public Release Release { get; set; }

        [JsonProperty("open_alerts")]
        public long OpenAlerts { get; set; }

        [JsonProperty("imei")]
        public string Imei { get; set; }
    }

}