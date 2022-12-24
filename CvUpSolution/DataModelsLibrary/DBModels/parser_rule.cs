using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class parser_rule
    {
        public int id { get; set; }
        public int parser_id { get; set; }
        public string delimiter { get; set; } = null!;
        public string value_type { get; set; } = null!;
        public int order { get; set; }
        public bool must_metch { get; set; }

        public virtual parser parser { get; set; } = null!;
    }
}
