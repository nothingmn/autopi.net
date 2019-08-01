using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using autopi.net.core.auth;
using Newtonsoft.Json;

namespace autopi.net.core.dongle
{
    public class DongleManager
    {
        private readonly HttpClient httpClient = AutoPiApiClient.Client;

        public async Task<IReadOnlyCollection<GetDongleResponse>> GetDongleDevices(string ordering = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var append = "";
            if (!string.IsNullOrEmpty(ordering)) append = "?ordering=" + ordering;
            var result = await httpClient.GetAsync("/dongle/devices/" + append);
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IReadOnlyCollection<GetDongleResponse>>(content);


        }
    }

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