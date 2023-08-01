using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class auth_out_email
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int user_id { get; set; }
        public DateTime sent_date { get; set; }
        public string to_address { get; set; } = null!;
        public string from_address { get; set; } = null!;
        public string subject { get; set; } = null!;
        public string? body { get; set; }
        public string? email_type { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual user user { get; set; } = null!;
    }
}
