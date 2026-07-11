using System;
using System.Collections.Generic;

namespace Database.models;

public partial class position
{
    public int id { get; set; }

    public int company_id { get; set; }

    public string name { get; set; } = null!;

    public string? descr { get; set; }

    public DateTime date_created { get; set; }

    public DateTime date_updated { get; set; }

    public int? customer_id { get; set; }

    public int? updater_id { get; set; }

    public int? opener_id { get; set; }

    public string status { get; set; } = null!;

    public int? cvdbid { get; set; }

    public string? remarks { get; set; }

    public string? requirements { get; set; }

    public int? position_number { get; set; }

    public int? assigned_user_id { get; set; }

    public int? contact_id { get; set; }

    public string? customer_pos_num { get; set; }

    public string? match_email_subject { get; set; }

    public int? cands_count { get; set; }

    public virtual company company { get; set; } = null!;

    public virtual contact? contact { get; set; }

    public virtual customer? customer { get; set; }

    public virtual ICollection<position_candidate> position_candidates { get; set; } = new List<position_candidate>();

    public virtual ICollection<position_contact> position_contacts { get; set; } = new List<position_contact>();

    public virtual ICollection<position_interviewer> position_interviewers { get; set; } = new List<position_interviewer>();
}
