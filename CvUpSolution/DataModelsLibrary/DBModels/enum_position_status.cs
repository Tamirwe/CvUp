using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class enum_position_status
    {
        public enum_position_status()
        {
            positions = new HashSet<position>();
        }

        public int id { get; set; }
        public string name { get; set; } = null!;

        public virtual ICollection<position> positions { get; set; }
    }
}
