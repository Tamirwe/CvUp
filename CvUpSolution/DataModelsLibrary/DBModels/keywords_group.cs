using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class keywords_group
    {
        public keywords_group()
        {
            keywords = new HashSet<keyword>();
        }

        public int id { get; set; }
        public int company_id { get; set; }
        public string name { get; set; } = null!;
        public DateTime updated { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual ICollection<keyword> keywords { get; set; }
    }
}
