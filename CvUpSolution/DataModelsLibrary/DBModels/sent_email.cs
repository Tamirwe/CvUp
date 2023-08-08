using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class sent_email
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public string? subject { get; set; }
        public string? body { get; set; }
        public int? position_id { get; set; }
        public int? candidate_id { get; set; }
        public int? cv_id { get; set; }
        public string? to { get; set; }
        public int user_id { get; set; }
        public DateTime date_sent { get; set; }

        public virtual company company { get; set; } = null!;
    }
}
