using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class folder
    {
        public folder()
        {
            folders_cands = new HashSet<folders_cand>();
        }

        public int id { get; set; }
        public int company_id { get; set; }
        public int parent_id { get; set; }
        public string name { get; set; } = null!;
        public int? cvdbid { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual ICollection<folders_cand> folders_cands { get; set; }
    }
}
