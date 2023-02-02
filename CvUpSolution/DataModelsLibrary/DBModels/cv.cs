using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class cv
    {
        public cv()
        {
            candidates = new HashSet<candidate>();
            cvs_txts = new HashSet<cvs_txt>();
            position_candidates = new HashSet<position_candidate>();
        }

        public int id { get; set; }
        public string? key_id { get; set; }
        public int company_id { get; set; }
        public int candidate_id { get; set; }
        public string? subject { get; set; }
        public string? from { get; set; }
        public string? email_id { get; set; }
        public long? cv_ascii_sum { get; set; }
        public string? position { get; set; }
        public int? duplicate_cv_id { get; set; }
        public DateTime date_created { get; set; }
        public string? pos_ids { get; set; }

        public virtual candidate candidate { get; set; } = null!;
        public virtual ICollection<candidate> candidates { get; set; }
        public virtual ICollection<cvs_txt> cvs_txts { get; set; }
        public virtual ICollection<position_candidate> position_candidates { get; set; }
    }
}
