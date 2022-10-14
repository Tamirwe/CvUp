using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class user
    {
        public user()
        {
            users_roles = new HashSet<users_role>();
        }

        public int id { get; set; }
        public int company_id { get; set; }
        public string email { get; set; } = null!;
        public string? passwaord { get; set; }
        public string first_name { get; set; } = null!;
        public string last_name { get; set; } = null!;

        public virtual company company { get; set; } = null!;
        public virtual ICollection<users_role> users_roles { get; set; }
    }
}
