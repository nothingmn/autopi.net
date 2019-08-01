using System;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace autopi.net.core.Models
{

    public partial class GetTripsResponse
    {
        public string StartDisplay
        {
            get
            {
                var road = (from s in StartPositionDisplay where !string.IsNullOrEmpty(s.Key) && s.Key.Equals("road") select s.Value)?.FirstOrDefault();
                var city = (from s in StartPositionDisplay where !string.IsNullOrEmpty(s.Key) && s.Key.Equals("city") select s.Value)?.FirstOrDefault();
                return road + " " + city;
            }
        }
        public string EndDisplay
        {
            get
            {
                var road = (from s in EndPositionDisplay where !string.IsNullOrEmpty(s.Key) && s.Key.Equals("road") select s.Value)?.FirstOrDefault();
                var city = (from s in EndPositionDisplay where !string.IsNullOrEmpty(s.Key) && s.Key.Equals("city") select s.Value)?.FirstOrDefault();
                return road + " " + city;
            }
        }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("start_time_utc")]
        public DateTimeOffset StartTimeUtc { get; set; }

        [JsonProperty("end_time_utc")]
        public DateTimeOffset EndTimeUtc { get; set; }

        [JsonProperty("start_position_lat")]
        public string StartPositionLat { get; set; }

        [JsonProperty("start_position_lng")]
        public string StartPositionLng { get; set; }

        [JsonProperty("start_position_display")]
        public Dictionary<string, string> StartPositionDisplay { get; set; }

        [JsonProperty("end_position_lat")]
        public string EndPositionLat { get; set; }

        [JsonProperty("end_position_lng")]
        public string EndPositionLng { get; set; }

        [JsonProperty("end_position_display")]
        public Dictionary<string, string> EndPositionDisplay { get; set; }

        [JsonProperty("device")]
        public Guid Device { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        public TimeSpan DurationTS
        {
            get
            {
                return this.EndTimeUtc - this.StartTimeUtc;
            }
        }

        [JsonProperty("distanceKm")]
        public double DistanceKm { get; set; }
    }
}