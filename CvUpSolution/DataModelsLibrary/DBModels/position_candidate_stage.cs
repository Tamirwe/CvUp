using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class position_candidate_stage
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public string name { get; set; } = null!;
        public int? order { get; set; }

        public virtual company company { get; set; } = null!;
    }
}
