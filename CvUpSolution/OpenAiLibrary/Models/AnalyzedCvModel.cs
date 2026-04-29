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
        [JsonProperty("seniority")] public string Seniority { get; set; } = "Unknown";
        [JsonProperty("years_experience")] public int? YearsExperience { get; set; }
        [JsonProperty("current_title_en")] public string? CurrentTitleEn { get; set; }
        [JsonProperty("current_title_he")] public string? CurrentTitleHe { get; set; }
        [JsonProperty("companies")] public List<string> Companies { get; set; } = [];
        [JsonProperty("skills")] public List<string> Skills { get; set; } = [];
        [JsonProperty("summary_en")] public string SummaryEn { get; set; } = "";
        [JsonProperty("summary_he")] public string SummaryHe { get; set; } = "";
        [JsonProperty("languages")] public string? Languages { get; set; }


        [Newtonsoft.Json.JsonIgnore] public CvLanguage CvLanguage { get; set; }
        [Newtonsoft.Json.JsonIgnore] public string? Region { get; set; }
        [Newtonsoft.Json.JsonIgnore] public string? Area { get; set; }
        [Newtonsoft.Json.JsonIgnore] public int CandidateId { get; set; }
        [Newtonsoft.Json.JsonIgnore] public int CvId { get; set; }
    }
}
