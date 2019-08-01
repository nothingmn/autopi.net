using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using Afk.ZoneInfo;
using System.Net.Http.Headers;
using autopi.net.core.Models;

namespace autopi.net.core.auth.API
{
    public class AuthManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public AuthManager(HttpClient httpClient, ILogger logger)
        {
            this._httpClient = httpClient;
            this._logger = logger;
        }
        public async Task<LoginResponse> CreateLogin(Credentials credentials, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (credentials == null || string.IsNullOrEmpty(credentials.Email) || string.IsNullOrEmpty(credentials.Password))
            {
                throw new System.ArgumentException("Invalid Credentials");
            }
            var argsAsJson = JsonConvert.SerializeObject(credentials);
            var contentPost = new StringContent(argsAsJson, System.Text.Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("/auth/login/", contentPost);
            var content = await result.Content.ReadAsStringAsync();

            _logger.Info("Create Login API Response:{0}", content);

            var response = JsonConvert.DeserializeObject<LoginResponse>(content);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Token);

            return response;
        }
    }
}