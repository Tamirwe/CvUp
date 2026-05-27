using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class user_activate_status_enum
    {
        public user_activate_status_enum()
        {
            users = new HashSet<user>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;

        public virtual ICollection<user> users { get; set; }
    }
}
