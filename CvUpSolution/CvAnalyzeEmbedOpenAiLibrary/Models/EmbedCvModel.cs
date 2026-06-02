using System;
using System.Collections.Generic;
using System.Text;

namespace CvAnalyzeEmbedOpenAiLibrary.Models
{
    public class EmbedCvModel111
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
        public List<string> JobsTitlesEn { get; set; } = [];
        public List<string> JobsTitlesHe { get; set; } = [];
        public List<string> ProfessionWordsEn { get; set; } = [];
        public List<string> ProfessionWordsHe { get; set; } = [];
        public List<string> ProfessionSkillsEn { get; set; } = [];
        public List<string> ProfessionSkillsHe { get; set; } = [];
        public string? Seniority { get; set; }
        public string? Education { get; set; }
        public List<string> Companies { get; set; } = [];
        public List<string>? Skills { get; set; } = [];
        public string? MilitaryService { get; set; }
        public string SummaryEn { get; set; } = "";
        public string SummaryHe { get; set; } = "";
        public int? YearsExperience { get; set; }
    }
}


