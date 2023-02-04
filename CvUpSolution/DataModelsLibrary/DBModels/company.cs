using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class company
    {
        public company()
        {
            candidates = new HashSet<candidate>();
            company_cvs_emails = new HashSet<company_cvs_email>();
            company_parsers = new HashSet<company_parser>();
            contacts = new HashSet<contact>();
            departments = new HashSet<department>();
            emails_sents = new HashSet<emails_sent>();
            hr_companies = new HashSet<hr_company>();
            hr_contacts = new HashSet<hr_contact>();
            position_candidate_stages = new HashSet<position_candidate_stage>();
            position_candidates = new HashSet<position_candidate>();
            position_hr_companies = new HashSet<position_hr_company>();
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

        public virtual ICollection<candidate> candidates { get; set; }
        public virtual ICollection<company_cvs_email> company_cvs_emails { get; set; }
        public virtual ICollection<company_parser> company_parsers { get; set; }
        public virtual ICollection<contact> contacts { get; set; }
        public virtual ICollection<department> departments { get; set; }
        public virtual ICollection<emails_sent> emails_sents { get; set; }
        public virtual ICollection<hr_company> hr_companies { get; set; }
        public virtual ICollection<hr_contact> hr_contacts { get; set; }
        public virtual ICollection<position_candidate_stage> position_candidate_stages { get; set; }
        public virtual ICollection<position_candidate> position_candidates { get; set; }
        public virtual ICollection<position_hr_company> position_hr_companies { get; set; }
        public virtual ICollection<position_interviewer> position_interviewers { get; set; }
        public virtual ICollection<position> positions { get; set; }
        public virtual ICollection<user> users { get; set; }
    }
}
