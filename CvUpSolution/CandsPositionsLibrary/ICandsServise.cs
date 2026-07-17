using Database.models;
using DataModelsLibrary.Models;
using LuceneLibrary;

namespace CandsPositionsLibrary
{
    public interface ICandsServise
    {
        Task<int> AddCv(ImportCvModel importCv);
        Task DeleteCv(int companyId, int candidateId, int cvId);
        Task DeleteCandidate(int companyId, int candidateId);
        Task UpdateCvKeyId(ImportCvModel importCv);
        Task<candidate?> GetCandidateByEmail(string email);
        Task<candidate?> GetCandidateByPhone(string phone);
        Task AddUpdateCandidateFromCvImport(ImportCvModel importCv);
        Task<CandModel?> GetCandidate(int companyId, int candId);
        Task<List<CandCvModel>> GetCandCvsList(int companyId, int candidateId);
        Task<CvModel?> GetCv(int cvId, int companyId);
        Task<cvs_txt?> CheckIsSameCv(int companyId, int candidateId, int cvAsciiSum);
        Task UpdateCandLastCv(int companyId, int candidateId, int cvId, bool isDuplicate, DateTime lastCvSent);
        Task UpdateCandLastCvSent(int candidateId, DateTime lastCvSent);
        Task UpdateCvDate(int cvId);
        Task AttachPosCandCv(AttachePosCandCvModel posCv);
        Task DetachPosCand(AttachePosCandCvModel posCv);
        Task UpdateCandPositionStatus(CandPosStageTypeUpdateModel posStatus);
        Task UpdatePosStageDate(CandPosStageTypeUpdateModel posStatus);
        Task RemovePosStage(CandPosStageTypeUpdateModel posStatus);
        Task<List<company_cvs_email>> GetCompaniesEmails();
        Task<List<SearchEntry>> SearchCands(int companyId, searchCandCvModel searchVals);
        Task<List<SearchEntry>> SearchForAiFilter( searchCandCvModel searchVals);
        Task UpdateCvsAsciiSum(int companyId);
        Task<List<CandPosStageTypeModel>> GetCandPosStagesTypes(int companyId);
        Task SendEmail(SendEmailModel emailData, UserModel? user);
        Task SaveCandReview(int companyId, CandReviewModel candReview);
        Task<List<EmailTemplateModel>> GetEmailTemplates(int companyId);
        Task AddUpdateEmailTemplate(EmailTemplateModel emailTemplate);
        Task DeleteEmailTemplate(int companyId, int id);
        Task UpdateCandDetails(CandDetailsModel candDetails);
        Task UpdateIsSeen(int companyId, int cvId);
        Task<List<CandReportModel?>> CandsReport(int companyId, string stageType);
        Task AddSendEmail(SendEmailModel emailData, int userId);
        Task SaveCustomerCandReview(int companyId, CandReviewModel customerCandReview);
        Task<List<SearchModel>> GetSearches(int companyId);
        Task SaveSearch(int companyId, SearchModel searchVals);
        Task StarSearch(int companyId, SearchModel searchVals);
        Task DeleteSearch(int companyId, SearchModel searchVals);
        Task DeleteAllNotStarSearches(int companyId);
        Task<List<keywordsGroupModel>> GetKeywordsGroups(int companyId);
        Task SaveKeywordsGroup(int companyId, keywordsGroupModel keywordsGroup);
        Task DeleteKeywordsGroup(int companyId, int id);
        Task<List<keywordModel>> GetKeywords(int companyId);
        Task SaveKeyword(int companyId, keywordModel keyword);
        Task DeleteKeyword(int companyId, int id);
        Task<List<blackCandModel>> GetBlackCandidatesList();
        Task UpdateBlackCandidateEmailCount(blackCandModel blackCand);
        Task AddBlackCand(int companyId, int candidateId);
        Task RemoveBlackCand(int candidateId);
        List<CandModel> MergeAiResultsWithCandsList(List<CandModel> candsList, List<AiCandidateSearchModel> aiResults);
    }
}
