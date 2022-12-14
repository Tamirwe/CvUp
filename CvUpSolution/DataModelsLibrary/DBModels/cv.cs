using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class cv
    {
        public cv()
        {
            cvs_txts = new HashSet<cvs_txt>();
            position_cvs = new HashSet<position_cv>();
        }

        public int id { get; set; }
        public string? key_id { get; set; }
        public int company_id { get; set; }
        public int candidate_id { get; set; }
        public string? subject { get; set; }
        public string? from { get; set; }
        public DateTime date_created { get; set; }
        public string? email_id { get; set; }
        public long? cv_ascii_sum { get; set; }

        public virtual candidate candidate { get; set; } = null!;
        public virtual ICollection<cvs_txt> cvs_txts { get; set; }
        public virtual ICollection<position_cv> position_cvs { get; set; }
    }
}
