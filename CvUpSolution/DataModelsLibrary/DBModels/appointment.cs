using System;
using System.Collections.Generic;

namespace Database.models;

public partial class appointment
{
    public int id { get; set; }

    public int company_id { get; set; }

    public int user_id { get; set; }

    public int? position_id { get; set; }

    public int? candidate_id { get; set; }

    public string subject { get; set; } = null!;

    public DateTime start_time { get; set; }

    public int duration_minutes { get; set; }

    public string? location { get; set; }

    public string? notes { get; set; }

    public string candidates { get; set; } = null!;

    public string contacts { get; set; } = null!;

    public string optional_participants { get; set; } = null!;

    public string ics_uid { get; set; } = null!;

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }
}
