using Database.models;
using DataModelsLibrary.Enums;
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
        public string name { get; set; } = string.Empty;
        public string customerName { get; set; } = string.Empty;
        public string? descr { get; set; } = string.Empty;
        public PositionStatusEnum status { get; set; } = PositionStatusEnum.Active;
        public DateTime? updated { get; set; }
        public int? customerId { get; set; } = 0;
        public int[] contactsIds { get; set; } = new int[] { };
        public int[] interviewersIds { get; set; } = new int[] { };
    }

    //public class PositionClientModel
    //{
    //    public int id { get; set; } = 0;
    //    public int companyId { get; set; } = 0;
    //    public string name { get; set; } = string.Empty;
    //    public string descr { get; set; } = string.Empty;
    //    public string status { get; set; } = string.Empty;
    //    public int customerId { get; set; } = 0;
    //    public int[] contactsIds { get; set; } = new int[] { };
    //    public int[] interviewersIds { get; set; } = new int[] { };
    //}

    public class FolderModel
    {
        public int id { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public int parentId { get; set; } = 0;
    }

    public class FolderCandidateModel
    {
        public int folderId { get; set; } = 0;
        public int candidateId { get; set; } = 0;
    }

    public class ContactModel
    {
        public int id { get; set; } = 0;
        public int? customerId { get; set; } = 0;
        public string firstName { get; set; } = string.Empty;
        public string? lastName { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string? phone { get; set; } = string.Empty;
        public string customerName { get; set; } = string.Empty;
    }
}
