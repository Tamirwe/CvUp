using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class company_parser
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int parser_id { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual parser parser { get; set; } = null!;
    }
}
