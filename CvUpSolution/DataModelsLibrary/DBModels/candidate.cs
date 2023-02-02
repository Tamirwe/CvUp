using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class candidate
    {
        public candidate()
        {
            cvs = new HashSet<cv>();
            position_candidates = new HashSet<position_candidate>();
        }

        public int id { get; set; }
        public int company_id { get; set; }
        public string? email { get; set; }
        public string? name { get; set; }
        public string? phone { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_updated { get; set; }
        public sbyte? has_duplicates_cvs { get; set; }
        public string? review_html { get; set; }
        public string? review_text { get; set; }
        public DateTime? last_cv_sent { get; set; }
        public int? last_cv_id { get; set; }
        public string? pos_ids { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual cv? last_cv { get; set; }
        public virtual ICollection<cv> cvs { get; set; }
        public virtual ICollection<position_candidate> position_candidates { get; set; }
    }
}
