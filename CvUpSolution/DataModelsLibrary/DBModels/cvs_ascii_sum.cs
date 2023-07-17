using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class cvs_ascii_sum
    {
        public int id { get; set; }
        public int? ascii_sum { get; set; }
        public int? cv_id { get; set; }
        public int? candidate_id { get; set; }
        public int? company_id { get; set; }
        public int? cvdbid { get; set; }
        public string? cv_folder { get; set; }
        public string? file_extension { get; set; }
        public int? file_type_key { get; set; }
        public DateTime? mail_date { get; set; }
        public int? year { get; set; }
        public int? month { get; set; }
        public string? cv_key { get; set; }
    }
}
