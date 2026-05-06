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
        [JsonProperty("city_he")] public string? CityHe { get; set; }
        [JsonProperty("languages")] public string? Languages { get; set; }
        [JsonProperty("work_experience")] public List<string> WorkExperience { get; set; } = [];
        [JsonProperty("profession_words")] public List<string> ProfessionWords { get; set; } = [];
        [JsonProperty("profession_skills")] public List<string> ProfessionSkills { get; set; } = [];
        [JsonProperty("seniority")] public string? Seniority { get; set; }
        [JsonProperty("education_he")] public List<string> Education { get; set; } = [];
        [JsonProperty("companies")] public List<string> Companies { get; set; } = [];
        [JsonProperty("skills")] public List<string> Skills { get; set; } = [];
        [JsonProperty("military_service_he")] public string? MilitaryService { get; set; }
        [JsonProperty("summary_en")] public string SummaryEn { get; set; } = "";
        [JsonProperty("summary_he")] public string SummaryHe { get; set; } = "";
        [JsonProperty("years_experience")] public int? YearsExperience { get; set; }


        [Newtonsoft.Json.JsonIgnore] public List<string> JobsTitlesEn { get; set; } = [];
        [Newtonsoft.Json.JsonIgnore] public List<string> JobsTitlesHe { get; set; } = [];
        [Newtonsoft.Json.JsonIgnore] public List<string> professionWordsEn { get; set; } = [];
        [Newtonsoft.Json.JsonIgnore] public List<string> professionWordsHe { get; set; } = [];
        [Newtonsoft.Json.JsonIgnore] public List<string> professionSkillsEn { get; set; } = [];
        [Newtonsoft.Json.JsonIgnore] public List<string> professionSkillsHe { get; set; } = [];


        [Newtonsoft.Json.JsonIgnore] public string? Region { get; set; }
        [Newtonsoft.Json.JsonIgnore] public string? Area { get; set; }
        [Newtonsoft.Json.JsonIgnore] public int CandidateId { get; set; }
        [Newtonsoft.Json.JsonIgnore] public int CvId { get; set; }
        [Newtonsoft.Json.JsonIgnore] public string? CvLanguage { get; set; }

    }
}
