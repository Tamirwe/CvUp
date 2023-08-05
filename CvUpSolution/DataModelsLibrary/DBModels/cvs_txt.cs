using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class cvs_txt
    {
        public int id { get; set; }
        public int? candidate_id { get; set; }
        public int cv_id { get; set; }
        public int? company_id { get; set; }
        public string? cv_txt { get; set; }
        public int? ascii_sum { get; set; }
        public string? email_subject { get; set; }

        public virtual cv cv { get; set; } = null!;
    }
}
