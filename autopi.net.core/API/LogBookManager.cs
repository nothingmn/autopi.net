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

        public async Task<IReadOnlyCollection<GetTripsResponse>> GetTrips(System.Guid device, string ordering = "-start_time_utc", DateTimeOffset? start = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(ordering)) ordering = "-start_time_utc";
            if (start == null || start >= DateTimeOffset.UtcNow) start = DateTimeOffset.UtcNow.AddYears(-2);

            var result = await _httpClient.GetAsync(
                string.Format("/logbook/trips/?device={0}&ordering={1}&start_time_utc__gte={2}", device.ToString(), ordering, start?.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss"))
                );

            var content = await result.Content.ReadAsStringAsync();

            if (content.Contains(":[]"))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<IReadOnlyCollection<GetTripsResponse>>(content);
        }

        public async Task<IReadOnlyCollection<PrimitiveFieldsResponse>> GetStorageRead(System.Guid device, DateTimeOffset from, string field_type = "primitive", string field = "event.vehicle.battery.level", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (from == null) from = DateTimeOffset.UtcNow.AddDays(-1);
            var result = await _httpClient.GetAsync(string.Format("/logbook/storage/read/?device_id={0}&from_utc={1}&field_type={2}&field={3}", device, from.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss"), field_type, field));
            var content = await result.Content.ReadAsStringAsync();
            //_logger.Info("Get Raw LogBook API Response:{0}", content);
            if (content.Contains(":[]"))
            {
                return null;
            }

            var pfResponse = JsonConvert.DeserializeObject<IReadOnlyCollection<PrimitiveFieldsResponse>>(content);
            foreach (var pf in pfResponse)
            {
                pf.Field = field;
            }
            return pfResponse;
        }
        public async Task<IReadOnlyCollection<PrimitiveFieldsResponse>> GetStorageRaw(System.Guid device, DateTimeOffset from, string data_type = "event.vehicle.battery.level", string field = "event.vehicle.battery.level", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (from == null) from = DateTimeOffset.UtcNow.AddDays(-1);
            var result = await _httpClient.GetAsync(string.Format("/logbook/storage/raw/?device_id={0}&data_type={2}", device, from.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss"), data_type, field));
            //var result = await _httpClient.GetAsync(string.Format("/logbook/storage/raw/?device_id={0}&from_utc={1}&field_type={2}&field={3}", device, from_utc.ToString("yyyy-MM-ddThh:mm:ss"), field_type, field));
            var content = await result.Content.ReadAsStringAsync();
            //_logger.Info("Get Raw LogBook API Response:{0}", content);

            if (content.Contains(":[]"))
            {
                return null;
            }

            var pfResponse = JsonConvert.DeserializeObject<IReadOnlyCollection<PrimitiveFieldsResponse>>(content);
            foreach (var pf in pfResponse)
            {
                pf.Field = field;
            }
            return pfResponse;
        }
        public async Task<IReadOnlyCollection<StorageField>> GetStorageFields(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _httpClient.GetAsync("/logbook/storage/fields/");
            var content = await result.Content.ReadAsStringAsync();
            if (content.Contains(":[]"))
            {
                return null;
            }

            //_logger.Info("Get LogBook Storage Fields API Response:{0}", content);
            return JsonConvert.DeserializeObject<IReadOnlyCollection<StorageField>>(content);
        }


        public async Task<IReadOnlyCollection<StorageDataResponse>> GetStorageData(Guid deviceId, DateTimeOffset? start = null, DateTimeOffset? end = null, string type = "position", string key = "position", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (start == null || start >= DateTimeOffset.UtcNow) start = DateTimeOffset.UtcNow.AddDays(-1);
            if (end == null || end >= DateTimeOffset.UtcNow) end = DateTimeOffset.UtcNow;

            var result = await _httpClient.GetAsync(
                string.Format("/logbook/storage/data/?type={0}&key={1}&device_id={2}&start_utc={3}&end_utc={4}", type, key, deviceId, start.Value.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss"), end.Value.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss"))
            );
            var content = await result.Content.ReadAsStringAsync();
            //_logger.Info("Get LogBook Storage Data API Response:{0}", content);
            return JsonConvert.DeserializeObject<IReadOnlyCollection<StorageDataResponse>>(content);
        }

        public async Task<RecentStatusResponse> GetRecentStats(Guid deviceId, DateTimeOffset? start = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (start == null || start >= DateTimeOffset.UtcNow) start = DateTimeOffset.UtcNow.AddDays(-1);

            var result = await _httpClient.GetAsync(
                string.Format(
                        "/logbook/recent_stats/?device_id={0}&from_timestamp={1}",
                        deviceId,
                        start.Value.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss")
                    )
            );
            var content = await result.Content.ReadAsStringAsync();
            //_logger.Info("Get LogBook Storage Data API Response:{0}", content);
            return JsonConvert.DeserializeObject<RecentStatusResponse>(content);
        }


        public async Task<LogBookEventsResponse> GetEvents(Guid deviceId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _httpClient.GetAsync(
                string.Format(
                        "/logbook/events/?device_id={0}",
                        deviceId
                    )
            );
            var content = await result.Content.ReadAsStringAsync();
            //_logger.Info("Get LogBook Events API Response:{0}", content);
            return LogBookEventsResponse.FromJson(content);
        }

        public async Task<MostRecentPositionResponse> GetMostRecentPosition(Guid deviceId, DateTimeOffset? start = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (start == null || start >= DateTimeOffset.UtcNow) start = DateTimeOffset.UtcNow.AddDays(-1);

            var result = await _httpClient.GetAsync(
                string.Format(
                        "/logbook/most_recent_position/?device_id={0}&from_timestamp={1}",
                        deviceId,
                        start.Value.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss")
                    )
            );
            var content = await result.Content.ReadAsStringAsync();
            //_logger.Info("Get LogBook Storage Data API Response:{0}", content);
            return JsonConvert.DeserializeObject<MostRecentPositionResponse>(content);
        }

    }
}