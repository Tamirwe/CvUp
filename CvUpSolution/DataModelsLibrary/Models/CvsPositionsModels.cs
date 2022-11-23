using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Models
{
    public class ImportCvModel
    {

        public string companyId { get; set; } = "";
        public string fileNamePath { get; set; } = "";
        public string fileExtension { get; set; } = "";
        public string cvTxt { get; set; } = "";
        public string phone { get; set; } = "";
        public string email { get; set; } = "";
        public int candidateId { get; set; }
        public string cvId { get; set; } = "";
        public string emailId { get; set; } = "";
        public string subject { get; set; } = "";
        public string from { get; set; } = "";
    }
}
