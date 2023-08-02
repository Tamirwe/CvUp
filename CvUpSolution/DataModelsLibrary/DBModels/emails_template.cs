using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class emails_template
    {
        public emails_template()
        {
            cand_stage_events = new HashSet<cand_stage_event>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;
        public string subject { get; set; } = null!;
        public string body { get; set; } = null!;
        public string? template_type { get; set; }

        public virtual ICollection<cand_stage_event> cand_stage_events { get; set; }
    }
}
