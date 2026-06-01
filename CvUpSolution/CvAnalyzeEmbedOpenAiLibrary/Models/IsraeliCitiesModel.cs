using Newtonsoft.Json;

namespace CvAnalyzeEmbedOpenAiLibrary.Models
{
    public class IsraeliCitiesModel
    {
        [JsonProperty("city")] public required string city { get; set; }
        [JsonProperty("region")] public required string region { get; set; }
        [JsonProperty("area")] public required string area { get; set; }
    }

}
