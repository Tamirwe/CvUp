using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class company
    {
        public company()
        {
            candidate_position_stages = new HashSet<candidate_position_stage>();
            candidates = new HashSet<candidate>();
            cvs = new HashSet<cv>();
            position_cvs = new HashSet<position_cv>();
            positions = new HashSet<position>();
            users = new HashSet<user>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;
        public string? descr { get; set; }
        public DateTime date_created { get; set; }
        public int activate_status_id { get; set; }
        public string? log_info { get; set; }

        public virtual enum_company_activate_status activate_status { get; set; } = null!;
        public virtual ICollection<candidate_position_stage> candidate_position_stages { get; set; }
        public virtual ICollection<candidate> candidates { get; set; }
        public virtual ICollection<cv> cvs { get; set; }
        public virtual ICollection<position_cv> position_cvs { get; set; }
        public virtual ICollection<position> positions { get; set; }
        public virtual ICollection<user> users { get; set; }
    }
}
