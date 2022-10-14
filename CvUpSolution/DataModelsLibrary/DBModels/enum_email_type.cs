using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class enum_email_type
    {
        public enum_email_type()
        {
            emails_sents = new HashSet<emails_sent>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;

        public virtual ICollection<emails_sent> emails_sents { get; set; }
    }
}
