using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class hr_company
    {
        public hr_company()
        {
            hr_contacts = new HashSet<hr_contact>();
            position_hr_companies = new HashSet<position_hr_company>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;
        public int company_id { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual ICollection<hr_contact> hr_contacts { get; set; }
        public virtual ICollection<position_hr_company> position_hr_companies { get; set; }
    }
}
