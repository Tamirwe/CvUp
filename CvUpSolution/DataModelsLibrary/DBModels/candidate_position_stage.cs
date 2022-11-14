using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class candidate_position_stage
    {
        public candidate_position_stage()
        {
            position_cvs = new HashSet<position_cv>();
        }

        public int id { get; set; }
        public int company_id { get; set; }
        public string name { get; set; } = null!;

        public virtual company company { get; set; } = null!;
        public virtual ICollection<position_cv> position_cvs { get; set; }
    }
}
