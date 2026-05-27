using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class futures_statistic
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public float? value { get; set; }
        public string descr { get; set; } = null!;
        public DateTime update_date { get; set; }
    }
}
