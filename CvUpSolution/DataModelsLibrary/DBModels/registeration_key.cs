using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class registeration_key
    {
        public string id { get; set; } = null!;
        public string email { get; set; } = null!;
        public int user_id { get; set; }
        public DateTime date_created { get; set; }
    }
}
