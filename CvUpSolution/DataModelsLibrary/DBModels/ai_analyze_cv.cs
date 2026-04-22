using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class ai_analyze_cv
    {
        public int id { get; set; }
        public int candidate_id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? city { get; set; }
        public string? region { get; set; }
        public string? area { get; set; }
        public string? summary { get; set; }
        public string? seniority { get; set; }
        public string? current_title { get; set; }
        public string? skills { get; set; }
        public int? years_experience { get; set; }
        public string? languages { get; set; }
        public DateTime? date_updated { get; set; }
        public DateTime? date_created { get; set; }
        public bool? is_embedded { get; set; }

        public virtual candidate candidate { get; set; } = null!;
    }
}
