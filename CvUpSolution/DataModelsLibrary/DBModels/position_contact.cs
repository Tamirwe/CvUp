using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class position_contact
    {
        public int id { get; set; }
        public int position_id { get; set; }
        public int contact_id { get; set; }
        public int company_id { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual contact contact { get; set; } = null!;
        public virtual position position { get; set; } = null!;
    }
}
