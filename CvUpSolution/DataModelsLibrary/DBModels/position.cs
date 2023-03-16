using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class position
    {
        public position()
        {
            position_candidates = new HashSet<position_candidate>();
            position_interviewers = new HashSet<position_interviewer>();
        }

        public int id { get; set; }
        public int company_id { get; set; }
        public string name { get; set; } = null!;
        public string? descr { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_updated { get; set; }
        public int? customer_id { get; set; }
        public sbyte is_active { get; set; }
        public int? updater_id { get; set; }
        public int opener_id { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual customer? customer { get; set; }
        public virtual user opener { get; set; } = null!;
        public virtual user? updater { get; set; }
        public virtual ICollection<position_candidate> position_candidates { get; set; }
        public virtual ICollection<position_interviewer> position_interviewers { get; set; }
    }
}
