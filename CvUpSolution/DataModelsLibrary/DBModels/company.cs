using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class company
    {
        public company()
        {
            candidate_position_stages = new HashSet<candidate_position_stage>();
            candidates = new HashSet<candidate>();
            contacts = new HashSet<contact>();
            departments = new HashSet<department>();
            hr_companies = new HashSet<hr_company>();
            hr_contacts = new HashSet<hr_contact>();
            position_cvs = new HashSet<position_cv>();
            position_hr_companies = new HashSet<position_hr_company>();
            position_users = new HashSet<position_user>();
            positions = new HashSet<position>();
            users = new HashSet<user>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;
        public string? descr { get; set; }
        public DateTime date_created { get; set; }
        public int activate_status_id { get; set; }
        public string? log_info { get; set; }
        public string? key_email { get; set; }
        public string? cvs_email { get; set; }

        public virtual enum_company_activate_status activate_status { get; set; } = null!;
        public virtual ICollection<candidate_position_stage> candidate_position_stages { get; set; }
        public virtual ICollection<candidate> candidates { get; set; }
        public virtual ICollection<contact> contacts { get; set; }
        public virtual ICollection<department> departments { get; set; }
        public virtual ICollection<hr_company> hr_companies { get; set; }
        public virtual ICollection<hr_contact> hr_contacts { get; set; }
        public virtual ICollection<position_cv> position_cvs { get; set; }
        public virtual ICollection<position_hr_company> position_hr_companies { get; set; }
        public virtual ICollection<position_user> position_users { get; set; }
        public virtual ICollection<position> positions { get; set; }
        public virtual ICollection<user> users { get; set; }
    }
}
