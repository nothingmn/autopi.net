using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace autopi.net.core.Models
{


    public partial class LogBookEventsResponse
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("results")]
        public Result[] Results { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("data")]
        public Datum[] Data { get; set; }

        [JsonProperty("ts")]
        public DateTimeOffset Ts { get; set; }

        [JsonProperty("area")]
        public Area Area { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("event.vehicle.battery.level", NullValueHandling = NullValueHandling.Ignore)]
        public long? EventVehicleBatteryLevel { get; set; }

        [JsonProperty("event.vehicle.obd.protocol", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? EventVehicleObdProtocol { get; set; }

        [JsonProperty("event.vehicle.obd.autodetected", NullValueHandling = NullValueHandling.Ignore)]
        public bool? EventVehicleObdAutodetected { get; set; }
    }

    public enum Area { VehicleBattery, VehicleEngine, VehicleObd, VehiclePosition };

    public partial class LogBookEventsResponse
    {
        public static LogBookEventsResponse FromJson(string json) => JsonConvert.DeserializeObject<LogBookEventsResponse>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this LogBookEventsResponse self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                AreaConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class AreaConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Area) || t == typeof(Area?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "vehicle/battery":
                    return Area.VehicleBattery;
                case "vehicle/engine":
                    return Area.VehicleEngine;
                case "vehicle/obd":
                    return Area.VehicleObd;
                case "vehicle/position":
                    return Area.VehiclePosition;
            }
            throw new Exception("Cannot unmarshal type Area");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Area)untypedValue;
            switch (value)
            {
                case Area.VehicleBattery:
                    serializer.Serialize(writer, "vehicle/battery");
                    return;
                case Area.VehicleEngine:
                    serializer.Serialize(writer, "vehicle/engine");
                    return;
                case Area.VehicleObd:
                    serializer.Serialize(writer, "vehicle/obd");
                    return;
                case Area.VehiclePosition:
                    serializer.Serialize(writer, "vehicle/position");
                    return;
            }
            throw new Exception("Cannot marshal type Area");
        }

        public static readonly AreaConverter Singleton = new AreaConverter();
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
