using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class hr_contact
    {
        public int id { get; set; }
        public int? contact_id { get; set; }
        public int? hr_company_id { get; set; }
        public int? company_id { get; set; }

        public virtual company? company { get; set; }
        public virtual contact? contact { get; set; }
        public virtual hr_company? hr_company { get; set; }
    }
}
