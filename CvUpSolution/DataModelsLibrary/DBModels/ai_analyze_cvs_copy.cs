using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class ai_analyze_cvs_copy
    {
        public int id { get; set; }
        public int candidate_id { get; set; }
        public int cv_id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? city { get; set; }
        public string? region { get; set; }
        public string? area { get; set; }
        public string? languages { get; set; }
        public string? current_job_title_en { get; set; }
        public string? current_job_title_he { get; set; }
        public string? profession_words_en { get; set; }
        public string? profession_words_he { get; set; }
        public string? profession_skills_en { get; set; }
        public string? profession_skills_he { get; set; }
        public string? seniority { get; set; }
        public string? education { get; set; }
        public string? companies { get; set; }
        public string? skills { get; set; }
        public string? military_service { get; set; }
        public string? summary_en { get; set; }
        public string? summary_he { get; set; }
        public int? years_experience { get; set; }
        public DateTime? date_updated { get; set; }
        public DateTime? date_created { get; set; }
        public bool? is_embedded { get; set; }
    }
}
