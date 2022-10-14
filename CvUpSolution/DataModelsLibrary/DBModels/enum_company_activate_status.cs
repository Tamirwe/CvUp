using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class enum_company_activate_status
    {
        public enum_company_activate_status()
        {
            companies = new HashSet<company>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;

        public virtual ICollection<company> companies { get; set; }
    }
}
