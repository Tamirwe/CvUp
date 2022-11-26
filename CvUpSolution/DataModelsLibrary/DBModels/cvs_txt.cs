using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class cvs_txt
    {
        public string cv_id { get; set; } = null!;
        public int? company_id { get; set; }
        public string? cv_txt { get; set; }

        public virtual cv cv { get; set; } = null!;
    }
}
