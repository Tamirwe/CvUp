using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class cand_stage_event
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int template_id { get; set; }
        public int? cand_pos_new_stage_id { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual emails_template template { get; set; } = null!;
    }
}
