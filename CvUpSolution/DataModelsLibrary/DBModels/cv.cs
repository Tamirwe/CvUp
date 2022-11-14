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

        public int id { get; set; }
        public int company_id { get; set; }
        public int candidate_id { get; set; }
        public string? cv1 { get; set; }
        public string? title { get; set; }
        public string? from { get; set; }
        public DateTime date_added { get; set; }
        public string? mail_id { get; set; }
        public string? file_extension { get; set; }

        public virtual candidate candidate { get; set; } = null!;
        public virtual company company { get; set; } = null!;
        public virtual ICollection<position_cv> position_cvs { get; set; }
    }
}
