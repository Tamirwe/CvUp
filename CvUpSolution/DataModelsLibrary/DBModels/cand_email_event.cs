using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class cand_email_event
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int email_type_id { get; set; }
        public int? new_stage_id { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual emails_template email_type { get; set; } = null!;
    }
}
