using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class candidate
    {
        public candidate()
        {
            cvs = new HashSet<cv>();
        }

        public int id { get; set; }
        public int company_id { get; set; }
        public string? opinion { get; set; }
        public string? email { get; set; }
        public string? name { get; set; }
        public string? phone { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_updated { get; set; }
        public sbyte? has_duplicates_cvs { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual ICollection<cv> cvs { get; set; }
    }
}
