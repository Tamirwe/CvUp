using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class cand_pos_stage
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public string name { get; set; } = null!;
        public int? order { get; set; }
        public string stage_Type { get; set; } = null!;
        public sbyte is_custom { get; set; }
        public string? color { get; set; }
        public string? stage_event { get; set; }

        public virtual company company { get; set; } = null!;
    }
}
