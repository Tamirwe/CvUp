using System;
using System.Collections.Generic;

namespace Database.models;

public partial class contract_send
{
    public int id { get; set; }

    public string contract_template_name { get; set; } = null!;

    public string? contract_template_descr { get; set; }

    public string company_name { get; set; } = null!;

    public string? contact_name { get; set; }

    public string email_address { get; set; } = null!;

    public string? email_subject { get; set; }

    public string? email_body { get; set; }

    public DateTime date_sent { get; set; }

    public string? company_address { get; set; }

    public string? company_id_number { get; set; }
}
