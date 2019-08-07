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

        ///Unknown : "dataType" field.!  
        //What are the possible values?
        // public async Task<IReadOnlyCollection<GetTripsResponse>> GetRaw(System.Guid device, string dataType = "system/power/on", CancellationToken cancellationToken = default(CancellationToken))
        // {
        //     var result = await _httpClient.GetAsync(string.Format("/logbook/raw/?device_id={0}&data_type={1}", device, dataType));
        //     var content = await result.Content.ReadAsStringAsync();
        //     _logger.Info("Get Raw LogBook API Response:{0}", content);

        //     return JsonConvert.DeserializeObject<IReadOnlyCollection<GetTripsResponse>>(content);
        // }

        public async Task<IReadOnlyCollection<PrimitiveFieldsResponse>> GetStorageRead(System.Guid device, DateTime from_utc, string field_type = "primitive", string field = "event.vehicle.battery.level", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (from_utc == null) from_utc = DateTime.Now.AddDays(-1);
            var result = await _httpClient.GetAsync(string.Format("/logbook/storage/read/?device_id={0}&from_utc={1}&field_type={2}&field={3}", device, from_utc.ToString("yyyy-MM-ddThh:mm:ss"), field_type, field));
            var content = await result.Content.ReadAsStringAsync();
            _logger.Info("Get Raw LogBook API Response:{0}", content);

            var pfResponse = JsonConvert.DeserializeObject<IReadOnlyCollection<PrimitiveFieldsResponse>>(content);
            foreach (var pf in pfResponse)
            {
                pf.Field = field;
            }
            return pfResponse;
        }
        public async Task<IReadOnlyCollection<PrimitiveFieldsResponse>> GetStorageRaw(System.Guid device, DateTime from_utc, string data_type = "event.vehicle.battery.level", string field = "event.vehicle.battery.level", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (from_utc == null) from_utc = DateTime.Now.AddDays(-1);
            var result = await _httpClient.GetAsync(string.Format("/logbook/storage/raw/?device_id={0}&data_type={2}", device, from_utc.ToString("yyyy-MM-ddThh:mm:ss"), data_type, field));
            //var result = await _httpClient.GetAsync(string.Format("/logbook/storage/raw/?device_id={0}&from_utc={1}&field_type={2}&field={3}", device, from_utc.ToString("yyyy-MM-ddThh:mm:ss"), field_type, field));
            var content = await result.Content.ReadAsStringAsync();
            _logger.Info("Get Raw LogBook API Response:{0}", content);

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
            _logger.Info("Get LogBook Storage Fields API Response:{0}", content);
            return JsonConvert.DeserializeObject<IReadOnlyCollection<StorageField>>(content);
        }


        public async Task<IReadOnlyCollection<StorageDataResponse>> GetStorageData(Guid deviceId, DateTimeOffset? start = null, DateTimeOffset? end = null, string type = "position", string key = "position", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (start == null || start >= DateTimeOffset.UtcNow) start = DateTimeOffset.UtcNow.AddDays(-1);
            if (end == null || end >= DateTimeOffset.UtcNow) end = DateTimeOffset.UtcNow;

            var result = await _httpClient.GetAsync(
                string.Format("/logbook/storage/data/?type={0}&key={1}&device_id={2}&start_utc={3}&end_utc={4}", type, key, deviceId, start, end)
            );
            var content = await result.Content.ReadAsStringAsync();
            _logger.Info("Get LogBook Storage Data API Response:{0}", content);
            return JsonConvert.DeserializeObject<IReadOnlyCollection<StorageDataResponse>>(content);
        }

    }
    public enum AutoPiEvents
    {
        event_system_power_reason,

    }
}