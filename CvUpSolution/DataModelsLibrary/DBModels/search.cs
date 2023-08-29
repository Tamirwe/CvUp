using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class search
    {
        public int id { get; set; }
        public int? company_id { get; set; }
        public string? val { get; set; }
        public string? advanced_val { get; set; }
        public bool? is_exact { get; set; }
        public DateTime? search_date { get; set; }
        public bool? is_starred { get; set; }

        public virtual company? company { get; set; }
    }
}
