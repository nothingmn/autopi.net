using System.Net.Http;

namespace autopi.net.core
{
    public class AutoPiApiClient
    {
        private static HttpClient _httpClient = null;

        static AutoPiApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new System.Uri("https://api.autopi.io/", System.UriKind.RelativeOrAbsolute);
        }

        public static HttpClient Client { get { return _httpClient; } }
    }
}