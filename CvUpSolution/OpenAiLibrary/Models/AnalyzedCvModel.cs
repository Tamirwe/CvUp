using Newtonsoft.Json;
using OpenAiLibrary.AnalyzeCvsAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiLibrary.Models
{
    public class AnalyzedCvModel
    {
        [JsonProperty("name")] public string? Name { get; set; }
        [JsonProperty("email")] public string? Email { get; set; }
        [JsonProperty("phone")] public string? Phone { get; set; }
        [JsonProperty("location")] public string? Location { get; set; }
        [JsonProperty("skills")] public List<string> Skills { get; set; } = [];
        [JsonProperty("seniority")] public string Seniority { get; set; } = "Unknown";
        [JsonProperty("years_experience")] public int? YearsExperience { get; set; }
        [JsonProperty("current_title")] public string? CurrentTitle { get; set; }
        [JsonProperty("languages")] public string? Languages { get; set; } 
        [JsonProperty("summary")] public string Summary { get; set; } = "";

        [Newtonsoft.Json.JsonIgnore] public CvLanguage CvLanguage { get; set; }
        [Newtonsoft.Json.JsonIgnore] public string? Region { get; set; }
        [Newtonsoft.Json.JsonIgnore] public string? Area { get; set; }
        [Newtonsoft.Json.JsonIgnore] public int CandidateId { get; set; }
    }
}
