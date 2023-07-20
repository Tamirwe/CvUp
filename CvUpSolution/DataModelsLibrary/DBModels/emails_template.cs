using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class emails_template
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public string subject { get; set; } = null!;
        public string body { get; set; } = null!;
    }
}
