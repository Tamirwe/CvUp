using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class roles_enum
    {
        public roles_enum()
        {
            users_roles = new HashSet<users_role>();
        }

        public int id { get; set; }
        public string? name { get; set; }

        public virtual ICollection<users_role> users_roles { get; set; }
    }
}
