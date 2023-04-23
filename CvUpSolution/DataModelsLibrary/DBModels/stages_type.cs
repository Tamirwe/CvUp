using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class stages_type
    {
        public int id { get; set; }
        public string stage_type { get; set; } = null!;
        public int order { get; set; }
        public string name { get; set; } = null!;
        public string? color { get; set; }
    }
}
