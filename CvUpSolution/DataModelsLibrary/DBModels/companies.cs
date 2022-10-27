using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class companies
    {
        public companies()
        {
            users = new HashSet<users>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;
        public string? descr { get; set; }
        public DateTime date_created { get; set; }
        public int activate_status_id { get; set; }
        public string? log_info { get; set; }

        public virtual enum_company_activate_status activate_status { get; set; } = null!;
        public virtual ICollection<users> users { get; set; }
    }
}
