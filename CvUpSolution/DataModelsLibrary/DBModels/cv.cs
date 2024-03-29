﻿using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class cv
    {
        public cv()
        {
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
        public string? position { get; set; }
        public int? duplicate_cv_id { get; set; }
        public DateTime date_created { get; set; }
        public string? pos_ids { get; set; }
        public int? cvdbid { get; set; }
        public string? file_extension { get; set; }
        public int? file_type { get; set; }
        public bool? is_seen { get; set; }
        public int? position_type_id { get; set; }

        public virtual candidate candidate { get; set; } = null!;
        public virtual position_type? position_type { get; set; }
        public virtual ICollection<cvs_txt> cvs_txts { get; set; }
        public virtual ICollection<position_candidate> position_candidates { get; set; }
    }
}
