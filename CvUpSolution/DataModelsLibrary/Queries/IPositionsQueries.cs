using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IPositionsQueries
    {
        //Task<List<CvsToIndexModel>> GetCompanyCvsToIndex(int companyId, int candidateId);
        Task<position> AddPosition(PositionModel data, int companyId, int userId);
        Task<position> UpdatePosition(PositionModel data, int companyId, int userId);
        Task<List<PositionModel>> GetPositionsList(int companyId);
        Task DeletePosition(int companyId, int id);
        Task<List<int>> getPositionContactsIds(int companyId, int positionId);
        Task<PositionModel> GetPosition(int positionId, int companyId);
        Task<List<ParserRulesModel>> GetParsersRules();
        Task<List<int>> GetCompaniesIds();
        Task<List<string?>> GetCompanyCvsIds(int companyId);
        Task<CvModel?> GetCv(int cvId, int companyId);
        Task<cvs_txt?> CheckIsSameCv(int companyId, int candidateId, int cvAsciiSum);
        Task AttachPosCandCv(AttachePosCandCvModel posCv);
        Task DetachPosCand(AttachePosCandCvModel posCv);
        Task UpdateCandPosArrays(int companyId, int candidateId);
        Task<List<company_cvs_email>> GetCompaniesEmails();
        Task AddUpdateInterviewers(int companyId, int positionId, int[] interviewersIds);
        Task AddUpdatePositionContacts(int companyId, int positionId, List<int>? contactsIds);
        Task<List<CandPosStageTypeModel>> GetCandPosStagesTypes(int companyId);
        Task<List<cv>> GetCompanyCvs(int companyId);
        Task SaveCandReview(int companyId, CandReviewModel candReview);
        Task<List<EmailTemplateModel>> GetEmailTemplates(int companyId);
        Task AddUpdateEmailTemplate(EmailTemplateModel emailTemplate);
        Task DeleteEmailTemplate(int companyId, int id);
        Task UpdateCandDetails(CandDetailsModel candDetails);
        Task UpdateCandPositionStatus(CandPosStageTypeUpdateModel posStatus);
        Task UpdatePosStageDate(CandPosStageTypeUpdateModel posStatus);
        Task RemovePosStage(CandPosStageTypeUpdateModel posStatus);
        Task UpdateIsSeen(int companyId, int cvId);
        Task<List<CandReportModel?>> CandsReport(int companyId, string stageType);
        Task<cand_pos_stage?> getPosStage(int companyId, string stageType);
        //Task updateCandPosCallEmailToCandidate(int companyId, int candidateId, int positionId);
        //Task updateCandPosEmailToCustomer(int companyId, int candidateId, int positionId);
        //Task updateCandPosRejectEmailToCandidate(int companyId, int candidateId, int positionId);
        Task UpdatePositionDate(int companyId, int positionId, bool isUpdateCount);
        Task<position?> GetPositionByMatchStr(int companyId, string matchStr);
        Task AddSendEmail(SendEmailModel emailData, int userId);
        Task<int?> GetPositionTypeId(int companyId, string positionRelated);
        Task<int> AddPositionTypeName(int companyId, string positionRelated);
        Task<List<PositionTypeModel>> GetPositionsTypes(int companyId);
        Task SaveCustomerCandReview(int companyId, CandReviewModel customerCandReview);
        Task UpdateCandCustomersReviews(int companyId, int candidateId);
        Task CalculatePositionTypesCount(int companyId);
        Task<List<PositionTypeCountModel>> PositionsTypesCvsCount(int companyId);
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
        Task SaveAnalyzedPosition(int positionId, AnalyzedPositionModel analyzedPosition, float[]? positionEmbedding);
        Task<int> GetPositionCompanyId(int positionId);
        Task<AnalyzedPositionModel?> GetAnalyzedPosition(int positionId);
        Task<SearchTermsModel?> GetExistPositionSearchTerms(int positionId, int id);
        Task SaveSearchTerms(SearchTermsModel searchTerms);
    }
}