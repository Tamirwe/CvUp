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
        public string? requirements { get; set; } = string.Empty;
        public PositionStatusEnum status { get; set; } = PositionStatusEnum.Active;
        public DateTime? updated { get; set; }
        public DateTime? created { get; set; }
        public int? customerId { get; set; } = 0;
        public List<int>? contactsIds { get; set; }
        public int[] interviewersIds { get; set; } = new int[] { };
        public string? emailsubjectAddon { get; set; } = string.Empty;
        public string? remarks { get; set; } = string.Empty;
        public string? matchEmailsubject { get; set; } = string.Empty;
        public int? candsCount { get; set; } = 0;
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

    public class TranslateModel
    {
        public string? txt { get; set; }
        public List<string>? txtList { get; set; }
        public string? lang { get; set; }
    }

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
        public string? role { get; set; } = string.Empty;
    }

    public class CustomerModel
    {
        public int id { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public string? address { get; set; } = string.Empty;
        public string? descr { get; set; } = string.Empty;
        public DateTime? created { get; set; }
    }

    public class FuturesOhlcModel
    {
        public int id { get; set; } = 0;
        public DateTime statisticDate { get; set; }
        public int open { get; set; } 
        public int high { get; set; } 
        public int low { get; set; } 
        public int close { get; set; }
        public int dayPoints { get; set; }

    }

    public class FuturesStatisticsModel
    {
        public int id { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public float? value { get; set; } = 0;
        public string descr { get; set; } = string.Empty;
        public DateTime updateDate { get; set; }
    }

    public class ModuleGenerateRequestModel
    {
        public string name { get; set; } = string.Empty;
    }

    public class ModuleGenerateResponseModel
    {
        public string path { get; set; } = string.Empty;
    }

    public class blackCandModel
    {
        public int id { get; set; }
        public int candidate_id { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public int? cvs_count { get; set; }
    }

    public class EmbedCvDataModel
    {
        public int CandidateId { get; set; }
        public int CvId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public string? Region { get; set; }
        public string? Area { get; set; }
        public string? Languages { get; set; }
        public string? CurrentTitleEn { get; set; }
        public string? CurrentTitleHe { get; set; }
        public string? Companies { get; set; }
        public List<string>? Skills { get; set; }
        public string SummaryEn { get; set; } = "";
        public string SummaryHe { get; set; } = "";
        public int? YearsExperience { get; set; }
        public string NormelizedHe { get; set; } = "";
        public string? Profession { get; set; }
        public string? Education { get; set; }
        public string? MilitaryService { get; set; }
        public string? Seniority { get; set; }
    }

    public class AiSearchResultModel
    {
        public ulong Id { get; set; }
        public float Score { get; set; }
        public string Name { get; set; } = "";
        public int CandidateId { get; set; }
        public required string CvId { get; set; }
        public string? Companies { get; set; }
        public string CurrentTitle { get; set; } = "";
        public string Location { get; set; } = "";
        public int YearsExperience { get; set; }
        public string Summary { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public List<string>? Skills { get; set; }
        public string? Profession { get; set; }
        public string? Education { get; set; }
        public string? MilitaryService { get; set; }
        public string? Seniority { get; set; }
    }

    public class SearchFilterModel
    {
        public string? Seniority { get; set; }  // "Senior", "Mid", "Junior", "Lead"
        public string? Location { get; set; }  // "תל אביב"
        public List<string>? RequiredSkills { get; set; }  // ["React", "C#"]
        public int? MinYearsExperience { get; set; }
        public int? MaxYearsExperience { get; set; }
    }
}
