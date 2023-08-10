using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface ICandsPositionsQueries
    {
         Task<int> AddCv(ImportCvModel importCv);
         Task<int> AddCandidate(candidate importCv);
         Task UpdateCandidate(candidate cand);
         Task<candidate?> GetCandidateByEmail(string email);
         Task<List<CvsToIndexModel>> GetCompanyCvsToIndex(int companyId, int candidateId);
        Task<CandModel?> GetCandidate(int companyId, int candId);
        Task<List<CandModel?>> GetCandsList(int companyId, string encriptKey, int page, int take, List<int>? candsIds);
        Task<List<CandModel?>> GetPosCandsList(int companyId, int positionId, List<int>? candsIds);
        Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId, List<int>? candsIds);
        Task<position> AddPosition(PositionModel data, int companyId, int userId);
         Task<position> UpdatePosition(PositionModel data, int companyId, int userId);
         Task<List<PositionModel>> GetPositionsList(int companyId);
         Task DeletePosition(int companyId, int id);
         Task<List<int>> getPositionContactsIds(int companyId, int positionId);
         Task<PositionModel> GetPosition(int companyId, int positionId);
         Task<List<ParserRulesModel>> GetParsersRules();
         Task<List<int>> GetCompaniesIds();
         Task<List<string?>> GetCompanyCvsIds(int companyId);
         Task<CvModel?> GetCv(int cvId, int companyId);
         Task UpdateCvKeyId(ImportCvModel importCv);
         Task<cvs_txt?> CheckIsSameCv(int companyId, int candidateId, int cvAsciiSum);
         Task UpdateCandLastCv(int companyId, int candidateId, int cvId, bool isDuplicate, DateTime lastCvSent);
         Task DeleteCv(int companyId, int candidateId, int cvId);
        Task<Tuple<cv?, bool>> GetCandLastCv(int companyId, int candidateId);
         Task DeleteCandidate(int companyId, int candidateId);
         Task UpdateSameCv(ImportCvModel importCv);
         Task<candidate?> GetCandidateByPhone(string phone);
         Task<List<CandCvModel>> GetCandCvsList(int companyId, int candidateId);
         Task UpdateCvsAsciiSum(int companyId);
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
        Task UpdateCandPositionStatus(CandPosStatusUpdateCvModel posStatus);
        Task UpdateIsSeen(int companyId, int cvId);
        Task<List<CandReportModel?>> CandsReport(int companyId, string stageType);
        Task<cand_pos_stage?> getPosStage(int companyId, string stageType);
        Task updateCandPosCallEmailToCandidate(int companyId, int candidateId, int positionId);
        Task updateCandPosEmailToCustomer(int companyId, int candidateId, int positionId);
        Task updateCandPosRejectEmailToCandidate(int companyId, int candidateId, int positionId);
        Task UpdatePositionDate(int companyId, int positionId);
        Task<position?> GetPositionByMatchStr(int companyId, string matchStr);
        Task AddSendEmail(SendEmailModel emailData, int userId);
    }
}