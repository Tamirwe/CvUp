using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class interviewer
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int user_id { get; set; }
        public int position_id { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual position position { get; set; } = null!;
        public virtual user user { get; set; } = null!;
    }
}
