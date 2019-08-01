using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using Afk.ZoneInfo;
using System.Net.Http.Headers;

namespace autopi.net.core.auth
{
    public class LoginManager
    {
        private readonly HttpClient httpClient = AutoPiApiClient.Client;

        public async Task<LoginResponse> CreateLogin(Credentials credentials, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (credentials == null || string.IsNullOrEmpty(credentials.Email) || string.IsNullOrEmpty(credentials.Password))
            {
                throw new System.ArgumentException("Invalid Credentials");
            }
            var argsAsJson = JsonConvert.SerializeObject(credentials);
            var contentPost = new StringContent(argsAsJson, System.Text.Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync("/auth/login/", contentPost);
            var content = await result.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<LoginResponse>(content);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Token);

            return response;
        }
    }


    public class Credentials
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }


    public partial class LoginResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }

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

    public partial class Addon
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("settings")]
        public Settings Settings { get; set; }
    }

    public partial class Settings
    {
    }

    public partial class Dashboard
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("default")]
        public bool Default { get; set; }
    }

    public partial class Device
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
        public string LastCommunication { get; set; }

        [JsonProperty("is_updated")]
        public bool IsUpdated { get; set; }

        [JsonProperty("release")]
        public Release Release { get; set; }

        [JsonProperty("open_alerts")]
        public long OpenAlerts { get; set; }

        [JsonProperty("imei")]
        public string Imei { get; set; }
    }

    public partial class Release
    {
        [JsonProperty("version")]
        public string Version { get; set; }
    }

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