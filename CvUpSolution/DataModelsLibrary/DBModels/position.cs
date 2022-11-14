using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class position
    {
        public position()
        {
            position_cvs = new HashSet<position_cv>();
        }

        public int id { get; set; }
        public int company_id { get; set; }
        public string name { get; set; } = null!;
        public string? descr { get; set; }
        public int status_id { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_updated { get; set; }
        public int opener_id { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual enum_position_status status { get; set; } = null!;
        public virtual ICollection<position_cv> position_cvs { get; set; }
    }
}
