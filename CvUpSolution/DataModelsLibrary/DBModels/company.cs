using System;
using System.Collections.Generic;

namespace Database.models;

public partial class company
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public string? descr { get; set; }

    public DateTime date_created { get; set; }

    public string? log_info { get; set; }

    public string active_status { get; set; } = null!;

    public virtual ICollection<auth_out_email> auth_out_emails { get; set; } = new List<auth_out_email>();

    public virtual ICollection<cand_pos_stage> cand_pos_stages { get; set; } = new List<cand_pos_stage>();

    public virtual ICollection<candidate> candidates { get; set; } = new List<candidate>();

    public virtual ICollection<company_cvs_email> company_cvs_emails { get; set; } = new List<company_cvs_email>();

    public virtual ICollection<company_parser> company_parsers { get; set; } = new List<company_parser>();

    public virtual ICollection<contact> contacts { get; set; } = new List<contact>();

    public virtual ICollection<customer> customers { get; set; } = new List<customer>();

    public virtual ICollection<folder> folders { get; set; } = new List<folder>();

    public virtual ICollection<folders_cand> folders_cands { get; set; } = new List<folders_cand>();

    public virtual ICollection<keyword> keywords { get; set; } = new List<keyword>();

    public virtual ICollection<keywords_group> keywords_groups { get; set; } = new List<keywords_group>();

    public virtual ICollection<position_candidate> position_candidates { get; set; } = new List<position_candidate>();

    public virtual ICollection<position_contact> position_contacts { get; set; } = new List<position_contact>();

    public virtual ICollection<position_interviewer> position_interviewers { get; set; } = new List<position_interviewer>();

    public virtual ICollection<position_type> position_types { get; set; } = new List<position_type>();

    public virtual ICollection<position> positions { get; set; } = new List<position>();

    public virtual ICollection<search> searches { get; set; } = new List<search>();

    public virtual ICollection<sent_email> sent_emails { get; set; } = new List<sent_email>();

    public virtual ICollection<user> users { get; set; } = new List<user>();

    public virtual ICollection<users_refresh_token> users_refresh_tokens { get; set; } = new List<users_refresh_token>();
}
