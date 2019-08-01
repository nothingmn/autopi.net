using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using autopi.net.core.Models;
using Newtonsoft.Json;

namespace autopi.net.core.API
{
    public class LogBookManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public LogBookManager(HttpClient httpClient, ILogger logger)
        {
            this._httpClient = httpClient;
            this._logger = logger;
        }

        public async Task<IReadOnlyCollection<GetTripsResponse>> GetTrips(System.Guid device, string ordering = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _httpClient.GetAsync("/logbook/trips/?device=" + device.ToString());
            var content = await result.Content.ReadAsStringAsync();
            _logger.Info("Get Trips API Response:{0}", content);

            return JsonConvert.DeserializeObject<IReadOnlyCollection<GetTripsResponse>>(content);
        }
    }
}