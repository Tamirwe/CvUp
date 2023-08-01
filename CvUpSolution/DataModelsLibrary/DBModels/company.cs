using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class company
    {
        public company()
        {
            auth_out_emails = new HashSet<auth_out_email>();
            candidates = new HashSet<candidate>();
            company_cvs_emails = new HashSet<company_cvs_email>();
            company_parsers = new HashSet<company_parser>();
            company_stages_types = new HashSet<company_stages_type>();
            contacts = new HashSet<contact>();
            customers = new HashSet<customer>();
            folders = new HashSet<folder>();
            folders_cands = new HashSet<folders_cand>();
            position_candidates = new HashSet<position_candidate>();
            position_contacts = new HashSet<position_contact>();
            position_interviewers = new HashSet<position_interviewer>();
            positions = new HashSet<position>();
            users = new HashSet<user>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;
        public string? descr { get; set; }
        public DateTime date_created { get; set; }
        public string? log_info { get; set; }
        public string active_status { get; set; } = null!;

        public virtual ICollection<auth_out_email> auth_out_emails { get; set; }
        public virtual ICollection<candidate> candidates { get; set; }
        public virtual ICollection<company_cvs_email> company_cvs_emails { get; set; }
        public virtual ICollection<company_parser> company_parsers { get; set; }
        public virtual ICollection<company_stages_type> company_stages_types { get; set; }
        public virtual ICollection<contact> contacts { get; set; }
        public virtual ICollection<customer> customers { get; set; }
        public virtual ICollection<folder> folders { get; set; }
        public virtual ICollection<folders_cand> folders_cands { get; set; }
        public virtual ICollection<position_candidate> position_candidates { get; set; }
        public virtual ICollection<position_contact> position_contacts { get; set; }
        public virtual ICollection<position_interviewer> position_interviewers { get; set; }
        public virtual ICollection<position> positions { get; set; }
        public virtual ICollection<user> users { get; set; }
    }
}
