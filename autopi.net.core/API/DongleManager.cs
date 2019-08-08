using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using autopi.net.core.Models;
using Newtonsoft.Json;

namespace autopi.net.core.API
{
    public class DongleManager
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger _logger;

        public DongleManager(HttpClient httpClient, ILogger logger)
        {
            this._httpClient = httpClient;
            this._logger = logger;
        }

        public async Task<IReadOnlyCollection<GetDongleResponse>> GetDongleDevices(string ordering = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var append = "";
            if (!string.IsNullOrEmpty(ordering)) append = "?ordering=" + ordering;
            var result = await _httpClient.GetAsync("/dongle/devices/" + append);
            var content = await result.Content.ReadAsStringAsync();
            _logger.Info("Get Dongles API Response:{0}", content);

            return JsonConvert.DeserializeObject<IReadOnlyCollection<GetDongleResponse>>(content);
        }
    }
}