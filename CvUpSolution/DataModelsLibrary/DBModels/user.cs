using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class user
    {
        public user()
        {
            emails_sents = new HashSet<emails_sent>();
        }

        public int id { get; set; }
        public int company_id { get; set; }
        public string email { get; set; } = null!;
        public string? passwaord { get; set; }
        public string first_name { get; set; } = null!;
        public string last_name { get; set; } = null!;
        public DateTime? date_created { get; set; }
        public int activate_status_id { get; set; }
        public string? log_info { get; set; }
        public int role { get; set; }

        public virtual enum_user_activate_status activate_status { get; set; } = null!;
        public virtual company company { get; set; } = null!;
        public virtual enum_role roleNavigation { get; set; } = null!;
        public virtual ICollection<emails_sent> emails_sents { get; set; }
    }
}
