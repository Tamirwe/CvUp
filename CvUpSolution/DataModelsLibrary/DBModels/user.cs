using System;
using System.Collections.Generic;

namespace Database.models;

public partial class user
{
    public int id { get; set; }

    public int company_id { get; set; }

    public string email { get; set; } = null!;

    public string? passwaord { get; set; }

    public string first_name { get; set; } = null!;

    public string last_name { get; set; } = null!;

    public DateTime? date_created { get; set; }

    public string? log_info { get; set; }

    public string permission_type { get; set; } = null!;

    public string active_status { get; set; } = null!;

    public string? phone { get; set; }

    public int? cvdbid { get; set; }

    public string? first_name_en { get; set; }

    public string? last_name_en { get; set; }

    public string? signature { get; set; }

    public string? mail_username { get; set; }

    public string? mail_password { get; set; }

    public virtual ICollection<auth_out_email> auth_out_emails { get; set; } = new List<auth_out_email>();

    public virtual company company { get; set; } = null!;

    public virtual ICollection<position_interviewer> position_interviewers { get; set; } = new List<position_interviewer>();

    public virtual ICollection<users_refresh_token> users_refresh_tokens { get; set; } = new List<users_refresh_token>();
}
