﻿using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class company
    {
        public company()
        {
            auth_out_emails = new HashSet<auth_out_email>();
            cand_pos_stages = new HashSet<cand_pos_stage>();
            candidates = new HashSet<candidate>();
            company_cvs_emails = new HashSet<company_cvs_email>();
            company_parsers = new HashSet<company_parser>();
            contacts = new HashSet<contact>();
            customers = new HashSet<customer>();
            folders = new HashSet<folder>();
            folders_cands = new HashSet<folders_cand>();
            position_candidates = new HashSet<position_candidate>();
            position_contacts = new HashSet<position_contact>();
            position_interviewers = new HashSet<position_interviewer>();
            positions = new HashSet<position>();
            sent_emails = new HashSet<sent_email>();
            users = new HashSet<user>();
            users_refresh_tokens = new HashSet<users_refresh_token>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;
        public string? descr { get; set; }
        public DateTime date_created { get; set; }
        public string? log_info { get; set; }
        public string active_status { get; set; } = null!;

        public virtual ICollection<auth_out_email> auth_out_emails { get; set; }
        public virtual ICollection<cand_pos_stage> cand_pos_stages { get; set; }
        public virtual ICollection<candidate> candidates { get; set; }
        public virtual ICollection<company_cvs_email> company_cvs_emails { get; set; }
        public virtual ICollection<company_parser> company_parsers { get; set; }
        public virtual ICollection<contact> contacts { get; set; }
        public virtual ICollection<customer> customers { get; set; }
        public virtual ICollection<folder> folders { get; set; }
        public virtual ICollection<folders_cand> folders_cands { get; set; }
        public virtual ICollection<position_candidate> position_candidates { get; set; }
        public virtual ICollection<position_contact> position_contacts { get; set; }
        public virtual ICollection<position_interviewer> position_interviewers { get; set; }
        public virtual ICollection<position> positions { get; set; }
        public virtual ICollection<sent_email> sent_emails { get; set; }
        public virtual ICollection<user> users { get; set; }
        public virtual ICollection<users_refresh_token> users_refresh_tokens { get; set; }
    }
}
