using System;
using System.Collections.Generic;

namespace Database.models;

public partial class search_term
{
    public int id { get; set; }

    public int position_id { get; set; }

    public List<string> must_have { get; set; } = null!;

    public List<string> should_have { get; set; } = null!;

    public List<string> must_have_in_result { get; set; } = null!;

    public List<string> should_have_in_result { get; set; } = null!;

    public string? ai_search_phrase { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public string? search_descr { get; set; }
}
