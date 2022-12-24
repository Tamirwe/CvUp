using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class parser
    {
        public parser()
        {
            company_parsers = new HashSet<company_parser>();
            parser_rules = new HashSet<parser_rule>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;

        public virtual ICollection<company_parser> company_parsers { get; set; }
        public virtual ICollection<parser_rule> parser_rules { get; set; }
    }
}
