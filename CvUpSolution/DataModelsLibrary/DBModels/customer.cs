using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class customer
    {
        public customer()
        {
            contacts = new HashSet<contact>();
            positions = new HashSet<position>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;
        public int company_id { get; set; }
        public DateTime? date_created { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual ICollection<contact> contacts { get; set; }
        public virtual ICollection<position> positions { get; set; }
    }
}
