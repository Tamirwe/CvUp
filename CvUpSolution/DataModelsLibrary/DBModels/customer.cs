using System;
using System.Collections.Generic;

namespace Database.models;

public partial class customer
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public int company_id { get; set; }

    public DateTime? date_created { get; set; }

    public string? address { get; set; }

    public string? descr { get; set; }

    public int? cvdbid { get; set; }

    public virtual company company { get; set; } = null!;

    public virtual ICollection<contact> contacts { get; set; } = new List<contact>();

    public virtual ICollection<position> positions { get; set; } = new List<position>();
}
