using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Models
{
   public class IdNameModel
    {
        public int id { get; set; }= 0;
        public string name { get; set; } = string.Empty;
    }

    public class PositionModel
    {
        public int id { get; set; } = 0;
        public int companyId { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public string descr { get; set; } = string.Empty;
        public bool isActive { get; set; }
        public DateTime updated { get; set; }
        public int departmentId { get; set; } = 0;
        public int[] hrCompaniesIds { get; set; } = new int[] { };
        public int[] interviewersIds { get; set; } = new int[] { };

    }

    //public class PositionListItemModel
    //{
    //    public int id { get; set; } = 0;
    //    public string name { get; set; } = string.Empty;
    //    public bool isActive { get; set; }
    //    public DateTime updated { get; set; }

    //}

    public class PositionClientModel
    {
        public int id { get; set; } = 0;
        public int companyId { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public string descr { get; set; } = string.Empty;
        public bool isActive { get; set; } = false;
        public int departmentId { get; set; } = 0;
        public int[] hrCompaniesIds { get; set; } = new int[] { };
        public int[] interviewersIds { get; set; } = new int[] { };
    }
}
