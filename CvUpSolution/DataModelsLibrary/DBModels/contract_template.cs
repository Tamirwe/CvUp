using System;
using System.Collections.Generic;

namespace Database.models;

public partial class contract_template
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public string? descr { get; set; }

    public DateTime date_created { get; set; }

    public DateTime date_updated { get; set; }

    public virtual ICollection<customer_contract> customer_contracts { get; set; } = new List<customer_contract>();
}
