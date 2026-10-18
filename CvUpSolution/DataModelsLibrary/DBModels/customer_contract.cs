using System;
using System.Collections.Generic;

namespace Database.models;

public partial class customer_contract
{
    public int id { get; set; }

    public int customer_id { get; set; }

    public int? contact_id { get; set; }

    public int? contract_template_id { get; set; }

    public string? descr { get; set; }

    public string? email_subject { get; set; }

    public string? email_body { get; set; }

    public DateTime? date_sent { get; set; }

    public DateTime date_created { get; set; }

    public virtual contact? contact { get; set; }

    public virtual contract_template? contract_template { get; set; }

    public virtual customer customer { get; set; } = null!;
}
