using System;
using System.Collections.Generic;

namespace Database.models;

public partial class analyzed_position
{
    public int id { get; set; }

    public int position_id { get; set; }

    public string? title { get; set; }

    public string? seniority { get; set; }

    public short? min_years_experience { get; set; }

    public string? degree_required { get; set; }

    public string? embedding_text { get; set; }

    public List<string> hard_requirements { get; set; } = null!;

    public List<string> skills_required { get; set; } = null!;

    public List<string> skills_preferred { get; set; } = null!;

    public List<string> industries { get; set; } = null!;

    public string languages { get; set; } = null!;

    public string lucene_keywords { get; set; } = null!;

    public DateTime analyzed_at { get; set; }

    public virtual position position { get; set; } = null!;
}
