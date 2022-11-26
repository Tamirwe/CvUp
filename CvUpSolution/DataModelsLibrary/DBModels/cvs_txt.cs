using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class cvs_txt
    {
        public string id { get; set; } = null!;
        public string? cv_txt { get; set; }

        public virtual cv idNavigation { get; set; } = null!;
    }
}
