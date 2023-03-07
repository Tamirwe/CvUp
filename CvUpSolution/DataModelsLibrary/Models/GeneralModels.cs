using Database.models;
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
        public int customerId { get; set; } = 0;
        public int[] hrCompaniesIds { get; set; } = new int[] { };
        public int[] interviewersIds { get; set; } = new int[] { };
    }

    public class PositionClientModel
    {
        public int id { get; set; } = 0;
        public int companyId { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public string descr { get; set; } = string.Empty;
        public bool isActive { get; set; } = false;
        public int customerId { get; set; } = 0;
        public int[] hrCompaniesIds { get; set; } = new int[] { };
        public int[] interviewersIds { get; set; } = new int[] { };
    }

    public class FolderModel
    {
        public int id { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public int parentId { get; set; } = 0;
    }

    public class ContactModel
    {
        public int id { get; set; } = 0;
        public int companyId { get; set; } = 0;
        public int? customerId { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string? phone { get; set; } = string.Empty;
    }
}
