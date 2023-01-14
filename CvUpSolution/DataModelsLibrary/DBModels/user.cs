using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class user
    {
        public user()
        {
            emails_sents = new HashSet<emails_sent>();
            position_interviewers = new HashSet<position_interviewer>();
            positionopeners = new HashSet<position>();
            positionupdaters = new HashSet<position>();
        }

        public int id { get; set; }
        public int company_id { get; set; }
        public string email { get; set; } = null!;
        public string? passwaord { get; set; }
        public string first_name { get; set; } = null!;
        public string last_name { get; set; } = null!;
        public DateTime? date_created { get; set; }
        public string? log_info { get; set; }
        public string? refresh_token { get; set; }
        public DateTime? refresh_token_expiry { get; set; }
        public string permission_type { get; set; } = null!;
        public string? active_status { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual ICollection<emails_sent> emails_sents { get; set; }
        public virtual ICollection<position_interviewer> position_interviewers { get; set; }
        public virtual ICollection<position> positionopeners { get; set; }
        public virtual ICollection<position> positionupdaters { get; set; }
    }
}
