using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class cvs_ascii_sum
    {
        public int id { get; set; }
        public int? ascii_sum { get; set; }
        public int? cv_id { get; set; }
        public int? candidate_id { get; set; }
        public int? company_id { get; set; }
        public int? cvdbid { get; set; }
    }
}
