using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiLibrary.Models
{
    internal class SearchFilterModel
    {
        public string? Seniority { get; set; }  // "Senior", "Mid", "Junior", "Lead"
        public string? Location { get; set; }  // "תל אביב"
        public List<string>? RequiredSkills { get; set; }  // ["React", "C#"]
        public int? MinYearsExperience { get; set; }
        public int? MaxYearsExperience { get; set; }
    }
}
