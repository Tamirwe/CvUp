using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class contact
    {
        public int id { get; set; }
        public int hr_company_id { get; set; }
        public string name { get; set; } = null!;
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? position { get; set; }
        public int company_id { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual hr_company hr_company { get; set; } = null!;
    }
}
