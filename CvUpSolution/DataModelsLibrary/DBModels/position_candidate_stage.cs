using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class position_candidate_stage
    {
        public int id { get; set; }
        public string stage_type { get; set; } = null!;
        public DateTime stage_date { get; set; }
        public int candidate_id { get; set; }
        public int position_candidate_id { get; set; }
        public int company_id { get; set; }

        public virtual candidate candidate { get; set; } = null!;
        public virtual company company { get; set; } = null!;
        public virtual position_candidate position_candidate { get; set; } = null!;
    }
}
