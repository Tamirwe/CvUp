using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class enum_user_activate_status
    {
        public enum_user_activate_status()
        {
            users = new HashSet<user>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;

        public virtual ICollection<user> users { get; set; }
    }
}
