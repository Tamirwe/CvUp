using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class enum_permission_type
    {
        public enum_permission_type()
        {
            users = new HashSet<user>();
        }

        public int id { get; set; }
        public string? name { get; set; }

        public virtual ICollection<user> users { get; set; }
    }
}
