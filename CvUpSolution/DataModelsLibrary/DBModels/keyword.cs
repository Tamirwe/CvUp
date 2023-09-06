using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class keyword
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public string? name_he { get; set; }
        public string? name_en { get; set; }
        public int group_id { get; set; }
        public DateTime updated { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual keywords_group group { get; set; } = null!;
    }
}
