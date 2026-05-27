using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class position_hr_company
    {
        public int id { get; set; }
        public int position_id { get; set; }
        public int hr_company_id { get; set; }
        public int company_id { get; set; }
        public DateTime? date_created { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual hr_company hr_company { get; set; } = null!;
        public virtual position position { get; set; } = null!;
    }
}
