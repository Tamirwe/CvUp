using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class roles_enum
    {
        public roles_enum()
        {
            users = new HashSet<user>();
        }

        public int id { get; set; }
        public string? name { get; set; }

        public virtual ICollection<user> users { get; set; }
    }
}
