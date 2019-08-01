using Afk.ZoneInfo;
using Newtonsoft.Json;
using System;

namespace autopi.net.core.Models
{


    public partial class User
    {
        [JsonProperty("pk")]
        public long Pk { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("countryISO")]
        public string CountryIso { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("streetNo")]
        public string StreetNo { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("is_verified")]
        public bool IsVerified { get; set; }

        [JsonProperty("has_devices")]
        public bool HasDevices { get; set; }

        [JsonProperty("has_pending_terms")]
        public bool HasPendingTerms { get; set; }

        [JsonProperty("devices")]
        public Device[] Devices { get; set; }

        [JsonProperty("groups")]
        public object[] Groups { get; set; }

        [JsonProperty("addons")]
        public Addon[] Addons { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        public DateTimeOffset ToLocalTime(DateTime dateTime)
        {
            return TzTimeZone.ToLocalTime(dateTime);
        }
        private TzTimeZone TzTimeZone
        {
            get
            {
                if (!string.IsNullOrEmpty(Timezone))
                {
                    return TzTimeInfo.GetZone(Timezone);
                }
                return null;
            }
        }

        [JsonProperty("use_metric_format")]
        public bool UseMetricFormat { get; set; }

        [JsonProperty("use_24_hour_format")]
        public bool Use24_HourFormat { get; set; }

        [JsonProperty("dashboards")]
        public Dashboard[] Dashboards { get; set; }
    }

}