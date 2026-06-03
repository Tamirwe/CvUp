using System;
using System.Collections.Generic;

namespace Database.models;

public partial class parser
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public virtual ICollection<company_parser> company_parsers { get; set; } = new List<company_parser>();

    public virtual ICollection<parser_rule> parser_rules { get; set; } = new List<parser_rule>();
}
