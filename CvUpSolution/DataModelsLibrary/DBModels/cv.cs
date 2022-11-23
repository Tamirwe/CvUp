using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class cv
    {
        public cv()
        {
            position_cvs = new HashSet<position_cv>();
        }

        public string id { get; set; } = null!;
        public int company_id { get; set; }
        public int candidate_id { get; set; }
        public string? cv_text { get; set; }
        public string? subject { get; set; }
        public string? from { get; set; }
        public DateTime date_added { get; set; }
        public string? email_id { get; set; }

        public virtual candidate candidate { get; set; } = null!;
        public virtual company company { get; set; } = null!;
        public virtual ICollection<position_cv> position_cvs { get; set; }
    }
}
