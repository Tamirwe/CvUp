using System;
using System.Collections.Generic;

namespace Database.models;

public partial class candidate
{
    public int id { get; set; }

    public int company_id { get; set; }

    public string? email { get; set; }

    public string? name { get; set; }

    public string? first_name { get; set; }

    public string? last_name { get; set; }

    public string? phone { get; set; }

    public string? adress { get; set; }

    public DateTime? date_created { get; set; }

    public DateTime? date_updated { get; set; }

    public bool has_duplicates_cvs { get; set; }

    public string? review { get; set; }

    public DateTime? review_date { get; set; }

    public DateTime? last_cv_sent { get; set; }

    public int? last_cv_id { get; set; }

    public string? pos_ids { get; set; }

    public string? pos_stages { get; set; }

    public int? cvdbid { get; set; }

    public string? folders_ids { get; set; }

    public string? customers_reviews { get; set; }

    public string? city { get; set; }

    public bool is_black_list { get; set; }

    public bool is_cv_analyzed { get; set; }

    public virtual ICollection<ai_analyze_cv> ai_analyze_cvs { get; set; } = new List<ai_analyze_cv>();

    public virtual ICollection<analyzed_cv> analyzed_cvs { get; set; } = new List<analyzed_cv>();

    public virtual company company { get; set; } = null!;

    public virtual ICollection<cv> cvs { get; set; } = new List<cv>();

    public virtual ICollection<cvs_txt> cvs_txts { get; set; } = new List<cvs_txt>();

    public virtual ICollection<folders_cand> folders_cands { get; set; } = new List<folders_cand>();

    public virtual ICollection<position_candidate> position_candidates { get; set; } = new List<position_candidate>();
}
