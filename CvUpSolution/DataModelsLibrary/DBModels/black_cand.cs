using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class black_cand
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string email { get; set; } = null!;
        public string? remarks { get; set; }
    }
}
