using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class company_cvs_email
    {
        public int id { get; set; }
        public string email { get; set; } = null!;
        public int company_id { get; set; }

        public virtual company company { get; set; } = null!;
    }
}
