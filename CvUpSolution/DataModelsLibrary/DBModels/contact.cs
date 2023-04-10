using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class contact
    {
        public contact()
        {
            position_contacts = new HashSet<position_contact>();
            positions = new HashSet<position>();
        }

        public int id { get; set; }
        public int company_id { get; set; }
        public int? customer_id { get; set; }
        public string first_name { get; set; } = null!;
        public string? last_name { get; set; }
        public string email { get; set; } = null!;
        public string? phone { get; set; }
        public int? cvdbid { get; set; }
        public string? role { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual customer? customer { get; set; }
        public virtual ICollection<position_contact> position_contacts { get; set; }
        public virtual ICollection<position> positions { get; set; }
    }
}
