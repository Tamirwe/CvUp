using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class refresh_token
    {
        public int user_id { get; set; }
        public string token { get; set; } = null!;
        public DateTime date_created { get; set; }
        public DateTime expiry_time { get; set; }

        public virtual user user { get; set; } = null!;
    }
}
