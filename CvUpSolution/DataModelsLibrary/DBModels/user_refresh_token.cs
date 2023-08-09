using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class user_refresh_token
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int user_id { get; set; }
        public string? token { get; set; }
        public DateTime? token_expire { get; set; }

        public virtual company company { get; set; } = null!;
        public virtual user user { get; set; } = null!;
    }
}
