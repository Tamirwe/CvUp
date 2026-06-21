using System;
using System.Collections.Generic;

namespace Database.models;

public partial class analyzed_cv
{
    public int id { get; set; }

    public int candidate_id { get; set; }

    public int cv_id { get; set; }

    public string? name { get; set; }

    public string? email { get; set; }

    public string? phone { get; set; }

    public string? city_he { get; set; }

    public string? region { get; set; }

    public string? area { get; set; }

    public short? estimate_age { get; set; }

    public short? years_experience { get; set; }

    public string? seniority_he { get; set; }

    public string? seniority_en { get; set; }

    public List<string>? languages { get; set; }

    public List<string>? skills { get; set; }

    public string? education { get; set; }

    public string? work_experience { get; set; }

    public string? profession_words { get; set; }

    public string? summary_he { get; set; }

    public string? summary_en { get; set; }

    public string? military_service_he { get; set; }

    public bool is_embedded { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual candidate candidate { get; set; } = null!;

    public virtual cv cv { get; set; } = null!;
}
