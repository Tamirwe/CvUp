using System;
using System.Collections.Generic;

namespace Database.models;

public partial class keywords_group
{
    public int id { get; set; }

    public int company_id { get; set; }

    public string name { get; set; } = null!;

    public DateTime updated { get; set; }

    public virtual company company { get; set; } = null!;

    public virtual ICollection<keyword> keywords { get; set; } = new List<keyword>();
}
