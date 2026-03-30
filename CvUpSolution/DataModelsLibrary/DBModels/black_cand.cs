using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class black_cand
    {
        public int id { get; set; }
        public int candidate_id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public int? cvs_count { get; set; }
        public string? remarks { get; set; }
        public DateTime? updated { get; set; }
    }
}
