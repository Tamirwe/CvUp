using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class enum_lung
    {
        public enum_lung()
        {
            emails_templates = new HashSet<emails_template>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;

        public virtual ICollection<emails_template> emails_templates { get; set; }
    }
}
