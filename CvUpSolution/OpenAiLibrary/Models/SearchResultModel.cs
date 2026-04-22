using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiLibrary.Models
{
    internal class SearchResultModel
    {
        public ulong Id { get; set; }
        public float Score { get; set; }
        public string FullName { get; set; } = "";
        public string CurrentTitle { get; set; } = "";
        public string Seniority { get; set; } = "";
        public string Location { get; set; } = "";
        public int YearsExperience { get; set; }
        public string SummaryHebrew { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public List<string> Skills { get; set; } = [];
    }
}
