using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataModelsLibrary.Models
{

    public class AnalyzedCvModel
    {
        [JsonProperty("name")] public string? Name { get; set; }
        [JsonProperty("estimate_age")] public int? EstimateAge { get; set; }
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

    public class CandidateSearchResultModel
    {
        public int candidateId { get; set; }
        public int cvId { get; set; }
        public double distance { get; set; }
        public string? name { get; set; }
        public string? city { get; set; }
        public string? jobsTitles { get; set; }
        public string? professionWords { get; set; }
        public int? age { get; set; }
        public string? education { get; set; }
        public string? companies { get; set; }
        public string? summary { get; set; }

       
    }

    public class AnalyzedCvsForEmbeedingModel
    {
        public int CandidateId { get; set; }
        public int CvId { get; set; }
        public string? Name { get; set; }
        public int? EstimateAge { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public string? Region { get; set; }
        public string? Area { get; set; }
        public string? Languages { get; set; }
        public List<string>? JobsTitlesEn { get; set; } = [];
        public List<string>? JobsTitlesHe { get; set; } = [];
        public List<string>? ProfessionWordsEn { get; set; } = [];
        public List<string>? ProfessionWordsHe { get; set; } = [];
        public List<string>? ProfessionSkillsEn { get; set; } = [];
        public List<string>? ProfessionSkillsHe { get; set; } = [];
        public string? Seniority { get; set; }
        public string? Education { get; set; }
        public List<string>? Companies { get; set; } = [];
        public List<string>? Skills { get; set; } = [];
        public string? MilitaryService { get; set; }
        public string SummaryEn { get; set; } = "";
        public string SummaryHe { get; set; } = "";
        public int? YearsExperience { get; set; }
    }
}
