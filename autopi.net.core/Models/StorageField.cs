using System;
using Newtonsoft.Json;

namespace autopi.net.core.Models
{
    public partial class StorageField
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("type")]
        public TypeEnum Type { get; set; }
    }

    public enum TypeEnum { Bool, Datetime, Float, Geo_Point, Long, String };
}