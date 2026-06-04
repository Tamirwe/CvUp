using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralLibrary.IsraelCities
{
    public class IsraeliCitiesModel
    {
        [JsonProperty("city")] public required string city { get; set; }
        [JsonProperty("region")] public required string region { get; set; }
        [JsonProperty("area")] public required string area { get; set; }
    }
}
