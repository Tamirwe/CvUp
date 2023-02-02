using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class position_candidate
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int position_id { get; set; }
        public int candidate_id { get; set; }
        public int cv_id { get; set; }
        public int? stage_id { get; set; }
        public DateTime date_created { get; set; }
        public string? cvs { get; set; }

        public virtual candidate candidate { get; set; } = null!;
        public virtual company company { get; set; } = null!;
        public virtual cv cv { get; set; } = null!;
        public virtual position position { get; set; } = null!;
        public virtual position_candidate_stage? stage { get; set; }
    }
}
