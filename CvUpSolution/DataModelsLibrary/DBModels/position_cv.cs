using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class position_cv
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int position_id { get; set; }
        public int cv_id { get; set; }
        public int candidate_stage_id { get; set; }
        public DateTime? date_created { get; set; }

        public virtual candidate_position_stage candidate_stage { get; set; } = null!;
        public virtual company company { get; set; } = null!;
        public virtual cv cv { get; set; } = null!;
        public virtual position position { get; set; } = null!;
    }
}
