using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using autopi.net.core.Models;
using Newtonsoft.Json;

namespace autopi.net.core.API
{

    public class AutomationManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public AutomationManager(HttpClient httpClient, ILogger logger)
        {
            this._httpClient = httpClient;
            this._logger = logger;
        }


        public async Task<IReadOnlyCollection<StorageField>> GetAutomationFields(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _httpClient.GetAsync("/automation/fields/");
            var content = await result.Content.ReadAsStringAsync();
            if (content.Contains(":[]"))
            {
                return null;
            }

            //_logger.Info("Get LogBook Storage Fields API Response:{0}", content);
            return JsonConvert.DeserializeObject<IReadOnlyCollection<StorageField>>(content);
        }

    }
}