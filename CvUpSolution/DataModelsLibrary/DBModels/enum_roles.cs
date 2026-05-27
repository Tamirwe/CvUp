using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class enum_roles
    {
        public enum_roles()
        {
            users = new HashSet<users>();
        }

        public int id { get; set; }
        public string? name { get; set; }

        public virtual ICollection<users> users { get; set; }
    }
}
