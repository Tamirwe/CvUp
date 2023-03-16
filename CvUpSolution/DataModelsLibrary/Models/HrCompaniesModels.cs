using Database.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Models
{
    public class HrCompanyModel
    {
        //public int Id { get; set; }
        public hr_company? hrCompany { get; set; }
        public contact[]? contacts { get; set; }
        
    }
}
