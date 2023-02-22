using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class folders_cand
    {
        public int id { get; set; }
        public int candidate_id { get; set; }
        public int folder_id { get; set; }
        public int company_id { get; set; }

        public virtual candidate candidate { get; set; } = null!;
        public virtual company company { get; set; } = null!;
        public virtual folder folder { get; set; } = null!;
    }
}
