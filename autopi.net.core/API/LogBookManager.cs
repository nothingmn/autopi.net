using System;
using System.Collections.Generic;
using System.Linq;
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


        public async Task<IReadOnlyCollection<StorageDataResponse>> GetStorageData(Guid deviceId, DateTimeOffset? start = null, DateTimeOffset? end = null, string type = "position", string key = "position", string interval = "1m", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (start == null || start >= DateTimeOffset.UtcNow) start = DateTimeOffset.UtcNow.AddDays(-1);
            if (end == null || end >= DateTimeOffset.UtcNow) end = DateTimeOffset.UtcNow;

            var result = await _httpClient.GetAsync(
                string.Format("/logbook/storage/data/?type={0}&key={1}&device_id={2}&start_utc={3}&end_utc={4}&interval={5}", type, key, deviceId, start.Value.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss"), end.Value.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss"), interval)
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

        public async Task<IReadOnlyCollection<StorageDataResponse>> GetPositionData(Guid deviceId, DateTimeOffset? start = null, DateTimeOffset? end = null, string interval = "1m", CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetStorageData(deviceId, start, end, type: "position", interval: interval, cancellationToken: cancellationToken);
        }
        public async Task<IReadOnlyCollection<StorageDataResponse>> GetSpeedData(Guid deviceId, DateTimeOffset? start = null, DateTimeOffset? end = null, string interval = "1m", CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetStorageData(deviceId, start, end, type: "primitive", key: "speed", interval: interval, cancellationToken);
        }
        public async Task<IReadOnlyCollection<StorageDataResponse>> GetEngineLoadLevelsData(Guid deviceId, DateTimeOffset? start = null, DateTimeOffset? end = null, string interval = "1m", CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetStorageData(deviceId, start, end, type: "primitive", key: "engine_load", interval: interval, cancellationToken);
        }
        public async Task<IReadOnlyCollection<StorageDataResponse>> GetEngineCoolantTemperatureData(Guid deviceId, DateTimeOffset? start = null, DateTimeOffset? end = null, string interval = "1m", CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetStorageData(deviceId, start, end, type: "primitive", key: "coolant_temp", interval: interval, cancellationToken);
        }
        public async Task<IReadOnlyCollection<StorageDataResponse>> GetRpiTemperatureData(Guid deviceId, DateTimeOffset? start = null, DateTimeOffset? end = null, string interval = "1m", CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetStorageData(deviceId, start, end, type: "primitive", key: "rpi-temperature", interval: interval, cancellationToken);
        }
        public async Task<IReadOnlyCollection<StorageDataResponse>> GetVoltageData(Guid deviceId, DateTimeOffset? start = null, DateTimeOffset? end = null, string interval = "1m", CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetStorageData(deviceId, start, end, type: "primitive", key: "voltage", interval: interval, cancellationToken);
        }
        public async Task<IReadOnlyCollection<StorageDataResponse>> GetFuelLevelData(Guid deviceId, DateTimeOffset? start = null, DateTimeOffset? end = null, string interval = "1m", CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetStorageData(deviceId, start, end, type: "primitive", key: "fuel_level", interval: interval, cancellationToken);
        }

        public async Task<IReadOnlyCollection<StorageDataResponse>> GetFuelRateData(Guid deviceId, DateTimeOffset? start = null, DateTimeOffset? end = null, string interval = "1m", CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetStorageData(deviceId, start, end, type: "primitive", key: "fuel_rate", interval: interval, cancellationToken);
        }
        public async Task<IReadOnlyCollection<StorageDataResponse>> GetIntakeTempData(Guid deviceId, DateTimeOffset? start = null, DateTimeOffset? end = null, string interval = "1m", CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetStorageData(deviceId, start, end, type: "primitive", key: "intake_temp", interval: interval, cancellationToken);
        }
        public async Task<TripData> GetTripData(Guid deviceId, PrimitiveDataPoints dataPoints, DateTimeOffset? start = null, DateTimeOffset? end = null, string interval = "1m", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (start == null || start >= DateTimeOffset.UtcNow) start = DateTimeOffset.UtcNow.AddDays(-1);
            if (end == null || end >= DateTimeOffset.UtcNow) end = DateTimeOffset.UtcNow;
            var data = new TripData()
            {
                Start = start.Value,
                End = end.Value,
                DeviceId = deviceId,
                PrimitiveDataPoints = dataPoints
            };

            if (dataPoints.HasFlag(PrimitiveDataPoints.Position))
            {
                data.Position = await GetPositionData(deviceId, start, end, interval, cancellationToken);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.CoolantTemp))
            {
                data.CoolantTemp = await GetEngineCoolantTemperatureData(deviceId, start, end, interval, cancellationToken);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.EngineLoad))
            {
                data.EngineLoad = await GetEngineLoadLevelsData(deviceId, start, end, interval, cancellationToken);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.FuelLevel))
            {
                data.FuelLevel = await GetFuelLevelData(deviceId, start, end, interval, cancellationToken);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.FuelRate))
            {
                data.FuelRate = await GetFuelRateData(deviceId, start, end, interval, cancellationToken);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.IntakeTemp))
            {
                data.IntakeTemp = await GetIntakeTempData(deviceId, start, end, interval, cancellationToken);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.RpiTemperature))
            {
                data.RpiTemperature = await GetRpiTemperatureData(deviceId, start, end, interval, cancellationToken);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.Speed))
            {
                data.Speed = await GetSpeedData(deviceId, start, end, interval, cancellationToken);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.Voltage))
            {
                data.Voltage = await GetVoltageData(deviceId, start, end, interval, cancellationToken);
            }

            return data;

        }
        public async Task<AlignedTripData> GetTripDataAligned(Guid deviceId, PrimitiveDataPoints dataPoints, DateTimeOffset? start = null, DateTimeOffset? end = null, string interval = "1m", CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = await GetTripData(deviceId, dataPoints, start, end, interval, cancellationToken);
            var aligned = new AlignedTripData()
            {
                DeviceId = deviceId,
                Start = start.Value,
                End = end.Value,
                PrimitiveDataPoints = dataPoints,
                AlignedDataPoints = new List<TripDataPoint>()
            };
            if (dataPoints.HasFlag(PrimitiveDataPoints.Voltage))
            {
                aligned.AlignedDataPoints = AlignData(aligned.AlignedDataPoints, data.Voltage, PrimitiveDataPoints.Voltage);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.CoolantTemp))
            {
                aligned.AlignedDataPoints = AlignData(aligned.AlignedDataPoints, data.CoolantTemp, PrimitiveDataPoints.CoolantTemp);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.EngineLoad))
            {
                aligned.AlignedDataPoints = AlignData(aligned.AlignedDataPoints, data.EngineLoad, PrimitiveDataPoints.EngineLoad);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.FuelLevel))
            {
                aligned.AlignedDataPoints = AlignData(aligned.AlignedDataPoints, data.FuelLevel, PrimitiveDataPoints.FuelLevel);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.FuelRate))
            {
                aligned.AlignedDataPoints = AlignData(aligned.AlignedDataPoints, data.FuelRate, PrimitiveDataPoints.FuelRate);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.IntakeTemp))
            {
                aligned.AlignedDataPoints = AlignData(aligned.AlignedDataPoints, data.IntakeTemp, PrimitiveDataPoints.IntakeTemp);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.Position))
            {
                aligned.AlignedDataPoints = AlignData(aligned.AlignedDataPoints, data.Position, PrimitiveDataPoints.Position);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.RpiTemperature))
            {
                aligned.AlignedDataPoints = AlignData(aligned.AlignedDataPoints, data.RpiTemperature, PrimitiveDataPoints.RpiTemperature);
            }
            if (dataPoints.HasFlag(PrimitiveDataPoints.Speed))
            {
                aligned.AlignedDataPoints = AlignData(aligned.AlignedDataPoints, data.Speed, PrimitiveDataPoints.Speed);
            }

            return aligned;
        }
        private List<TripDataPoint> AlignData(List<TripDataPoint> first, IReadOnlyCollection<StorageDataResponse> second, PrimitiveDataPoints dataPoint)
        {
            if (first == null) first = new List<TripDataPoint>();

            int threshold = 30;
            foreach (var newPoint in second)
            {
                var existing = (from f in first where Math.Abs((f.Timestamp - newPoint.Ts).TotalSeconds) < threshold select f)?.FirstOrDefault();
                if (existing == null)
                {
                    //datapoint isnt there yet, add it
                    existing = new TripDataPoint()
                    {
                        Timestamp = newPoint.Ts
                    };
                    first.Add(existing);

                }
                else
                {
                    if (dataPoint == PrimitiveDataPoints.CoolantTemp) existing.CoolantTemp = newPoint.Value;
                    if (dataPoint == PrimitiveDataPoints.EngineLoad) existing.EngineLoad = newPoint.Value;
                    if (dataPoint == PrimitiveDataPoints.FuelLevel) existing.FuelLevel = newPoint.Value;
                    if (dataPoint == PrimitiveDataPoints.FuelRate) existing.FuelRate = newPoint.Value;
                    if (dataPoint == PrimitiveDataPoints.IntakeTemp) existing.IntakeTemp = newPoint.Value;
                    if (dataPoint == PrimitiveDataPoints.Position) existing.Position = newPoint.Location;
                    if (dataPoint == PrimitiveDataPoints.RpiTemperature) existing.RpiTemperature = newPoint.Value;
                    if (dataPoint == PrimitiveDataPoints.Speed) existing.Speed = newPoint.Value;
                    if (dataPoint == PrimitiveDataPoints.Voltage) existing.Voltage = newPoint.Value;
                }
            }

            first = PolyFill(first.OrderBy(t => t.Timestamp));
            first = PolyFill(first.OrderByDescending(t => t.Timestamp));
            return first;

        }
        private List<TripDataPoint> PolyFill(IOrderedEnumerable<TripDataPoint> points)
        {

            //now that we have every datapoint merged, polyfill null or empty values
            TripDataPoint last = null;
            foreach (var point in points)
            {
                var p = point;
                if (last == null)
                {
                    last = p;
                    continue;
                }
                if (p.Position == null) p.Position = last.Position;
                if (p.CoolantTemp == 0) p.CoolantTemp = last.CoolantTemp;
                if (p.EngineLoad == 0) p.EngineLoad = last.EngineLoad;
                if (p.FuelLevel == 0) p.FuelLevel = last.FuelLevel;
                if (p.FuelRate == 0) p.FuelRate = last.FuelRate;
                if (p.IntakeTemp == 0) p.IntakeTemp = last.IntakeTemp;
                if (p.RpiTemperature == 0) p.RpiTemperature = last.RpiTemperature;
                if (p.Speed == 0) p.Speed = last.Speed;
                if (p.Voltage == 0) p.Voltage = last.Voltage;

                last = p;
            }
            return points.ToList();
        }
    }

    public class AlignedTripData
    {
        public Guid DeviceId { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public PrimitiveDataPoints PrimitiveDataPoints { get; set; }

        public List<TripDataPoint> AlignedDataPoints { get; set; }
    }
    public class TripDataPoint
    {
        public DateTimeOffset Timestamp { get; set; }
        public Location Position { get; set; }
        public float Speed { get; set; }
        public float EngineLoad { get; set; }
        public float CoolantTemp { get; set; }
        public float RpiTemperature { get; set; }
        public float Voltage { get; set; }
        public float FuelLevel { get; set; }
        public float FuelRate { get; set; }
        public float IntakeTemp { get; set; }
    }
    public class TripData
    {
        public Guid DeviceId { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public PrimitiveDataPoints PrimitiveDataPoints { get; set; }
        public IReadOnlyCollection<StorageDataResponse> Position { get; set; }
        public IReadOnlyCollection<StorageDataResponse> Speed { get; set; }
        public IReadOnlyCollection<StorageDataResponse> EngineLoad { get; set; }
        public IReadOnlyCollection<StorageDataResponse> CoolantTemp { get; set; }
        public IReadOnlyCollection<StorageDataResponse> RpiTemperature { get; set; }
        public IReadOnlyCollection<StorageDataResponse> Voltage { get; set; }
        public IReadOnlyCollection<StorageDataResponse> FuelLevel { get; set; }
        public IReadOnlyCollection<StorageDataResponse> FuelRate { get; set; }
        public IReadOnlyCollection<StorageDataResponse> IntakeTemp { get; set; }
    }

    [Flags]
    public enum PrimitiveDataPoints
    {
        Position = 1,
        Speed = 2,
        EngineLoad = 4,
        CoolantTemp = 8,
        RpiTemperature = 16,
        Voltage = 32,
        FuelLevel = 64,
        FuelRate = 128,
        IntakeTemp = 256
    }
}