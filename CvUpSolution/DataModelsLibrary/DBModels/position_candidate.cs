using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class position_candidate
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int position_id { get; set; }
        public int candidate_id { get; set; }
        public int cv_id { get; set; }
        public int? stage_id { get; set; }
        public string? stage_type { get; set; }
        public DateTime? email_to_contact { get; set; }
        public DateTime? call_email_to_candidate { get; set; }
        public DateTime? reject_email_to_candidate { get; set; }
        public DateTime? stage_date { get; set; }
        public DateTime date_created { get; set; }
        public DateTime? date_updated { get; set; }
        public string? cand_cvs { get; set; }
        public string? customer_review { get; set; }
        public DateTime? date_cv_sent_to_customer { get; set; }
        public DateTime? date_interview_customer_request { get; set; }
        public DateTime? date_sent_talk_request { get; set; }
        public DateTime? date_rejected { get; set; }
        public DateTime? date_accepted { get; set; }
        public DateTime? date_msg_accept_reject_sent { get; set; }
        public DateTime? date_remove_candidacy { get; set; }

        public virtual candidate candidate { get; set; } = null!;
        public virtual company company { get; set; } = null!;
        public virtual cv cv { get; set; } = null!;
        public virtual position position { get; set; } = null!;
    }
}
