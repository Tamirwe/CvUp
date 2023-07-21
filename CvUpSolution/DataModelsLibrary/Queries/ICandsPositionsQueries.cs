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
         Task<List<CvsToIndexModel>> GetCompanyCvsToIndex(int companyId);
         Task<List<CandModel?>> GetCandsList(int companyId, string encriptKey, int page, int take, List<int>? candsIds);
         Task<position> AddPosition(PositionModel data, int companyId, int userId);
         Task<position> UpdatePosition(PositionModel data, int companyId, int userId);
         Task<List<PositionModel>> GetPositionsList(int companyId);
         Task DeletePosition(int companyId, int id);
         Task<PositionModel> GetPosition(int companyId, int positionId);
         Task<List<ParserRulesModel>> GetParsersRules(int companyId);
         Task<List<int>> GetCompaniesIds();
         Task<List<string?>> GetCompanyCvsIds(int companyId);
         Task<CvModel?> GetCv(int cvId, int companyId);
         Task UpdateCvKeyId(ImportCvModel importCv);
         Task SaveCvReview(CvReviewModel cvReview);
         Task<List<cv>> CheckIsCvDuplicate(int companyId, int candidateId, int cvAsciiSum);
         Task UpdateCandidateLastCv(ImportCvModel importCv);
         Task UpdateSameCv(ImportCvModel importCv);
         Task<candidate?> GetCandidateByPhone(string phone);
         Task<List<CandCvModel>> GetCandCvsList(int companyId, int candidateId, string encriptKey);
         Task UpdateCvsAsciiSum(int companyId);
         Task<List<CandModel?>> GetPosCandsList(int companyId, int positionId, List<int>? candsIds);
         Task<CandPosModel> AttachPosCandCv(AttachePosCandCvModel posCv);
         Task<CandPosModel> DetachPosCand(AttachePosCandCvModel posCv);
         Task<List<company_cvs_email>> GetCompaniesEmails();
        Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId, List<int>? candsIds);
        Task ActivatePosition(int companyId, PositionModel data);
        Task DactivatePosition(int companyId, PositionModel data);
        Task AddUpdateInterviewers(int companyId, int positionId, int[] interviewersIds);
        Task AddUpdateContacts(int companyId, int positionId, int[] contactsIds);
        Task<List<companyStagesTypesModel>> GetCompanyStagesTypes(int companyId);
         Task<List<cv>> GetCompanyCvs(int companyId);
        Task SaveCandReview(CandReviewModel candReview);
        Task<List<EmailTemplateModel>> GetEmailTemplates(int companyId);
        Task AddUpdateEmailTemplate(EmailTemplateModel emailTemplate);
        Task DeleteEmailTemplate(int companyId, int id);
        Task UpdateCandDetails(CandDetailsModel candDetails);
    }
}