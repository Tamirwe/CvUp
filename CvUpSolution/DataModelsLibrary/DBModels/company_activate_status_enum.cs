using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class company_activate_status_enum
    {
        public company_activate_status_enum()
        {
            companies = new HashSet<company>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;

        public virtual ICollection<company> companies { get; set; }
    }
}
