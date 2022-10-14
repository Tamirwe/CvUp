using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class company
    {
        public company()
        {
            users = new HashSet<user>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;

        public virtual ICollection<user> users { get; set; }
    }
}
