using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class users_role
    {
        public int id { get; set; }
        public int? user_id { get; set; }
        public int? role_id { get; set; }

        public virtual roles_enum? role { get; set; }
        public virtual user? user { get; set; }
    }
}
