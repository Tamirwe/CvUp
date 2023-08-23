using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class position_type
    {
        public position_type()
        {
            cvs = new HashSet<cv>();
        }

        public int id { get; set; }
        public string type_name { get; set; } = null!;
        public DateTime date_updated { get; set; }
        public int company_id { get; set; }
        public int? cvs_today { get; set; }
        public int? cvs_yesterday { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual ICollection<cv> cvs { get; set; }
    }
}
