using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class position_interviewer
    {
        public int id { get; set; }
        public int? position_id { get; set; }
        public int? user_id { get; set; }
        public int? company_id { get; set; }

        public virtual company? company { get; set; }
        public virtual position? position { get; set; }
        public virtual user? user { get; set; }
    }
}
