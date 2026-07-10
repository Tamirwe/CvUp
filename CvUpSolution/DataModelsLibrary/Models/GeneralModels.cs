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
        public int id { get; set; } = 0;
        public string name { get; set; } = string.Empty;
    }

    public class PositionModel
    {
        public int id { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public string customerName { get; set; } = string.Empty;
        public string? descr { get; set; } = string.Empty;
        public string? requirements { get; set; } = string.Empty;
        public string? positionAd { get; set; } = string.Empty;
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

    public class blackCandModel
    {
        public int id { get; set; }
        public int candidate_id { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public int? cvs_count { get; set; }
    }



    public class AiSearchResultModel
    {
        public ulong Id { get; set; }
        public int CandidateId { get; set; }
        public float Score { get; set; }
        public string Name { get; set; } = "";
        public int? EstimateAge { get; set; }
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public required string CvId { get; set; }
        public string Location { get; set; } = "";
        public List<string> JobsTitlesHe { get; set; } = [];
        public List<string> ProfessionWords { get; set; } = [];
        public List<string> ProfessionSkills { get; set; } = [];
        public string? Seniority { get; set; }
        public string? Education { get; set; }
        public List<string> Companies { get; set; } = [];
        public List<string> Skills { get; set; } = [];
        public string? MilitaryService { get; set; }
        public string Summary { get; set; } = "";
        public int? YearsExperience { get; set; }
    }

    public class SearchFilterModel
    {
        public string? Seniority { get; set; }  // "Senior", "Mid", "Junior", "Lead"
        public string? Location { get; set; }  // "תל אביב"
        public string? Area { get; set; }  // "צפון, מרכז, דרום"
        public List<string>? RequiredSkills { get; set; }  // ["React", "C#"]
        public int? MinYearsExperience { get; set; }
        public int? MaxYearsExperience { get; set; }
    }

    public class SearchEntry
    {
        public int Id { get; set; }
        public string CV { get; set; } = string.Empty;
        public long UpdatedTs { get; set; }
        public DateTime Updated { get; set; }
        public int Score { get; set; }
    }

    public class PositionSearchTermsModel
    {
        public List<string> LuceneKeywords { get; set; } = [];
        public string? SearchPhrase { get; set; }
    }

    public class SearchTermsModel
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public List<string> MustHave { get; set; } = [];
        public List<string> ShouldHave { get; set; } = [];
        public List<string> MustHaveInResult { get; set; } = [];
        public List<string> ShouldHaveInResult { get; set; } = [];
        public string? AiSearchPhrase { get; set; }
    }

}
