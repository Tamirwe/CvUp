using System;
using System.Collections.Generic;

namespace Database.models;

public partial class job_queue
{
    public long id { get; set; }

    public string queue_name { get; set; } = null!;

    public string payload { get; set; } = null!;

    public string status { get; set; } = null!;

    public int attempts { get; set; }

    public DateTime created_at { get; set; }

    public DateTime visible_at { get; set; }

    public DateTime? locked_at { get; set; }

    public string? locked_by { get; set; }

    public int max_attempts { get; set; }
}
