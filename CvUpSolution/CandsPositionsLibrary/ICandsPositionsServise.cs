using Database.models;
using DataModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandsPositionsLibrary
{
    public interface ICandsPositionsServise
    {
        Task<int> AddCv(ImportCvModel importCv);
        Task AddUpdateCandidateFromCvImport(ImportCvModel importCv);
        Task IndexCompanyCvs(int companyId);
        Task<List<CandModel?>> GetCandsList(int companyId, int page, int take, List<int>? candsIds);
        Task<int> AddPosition(PositionModel data, int companyId, int userId);
        Task<int> UpdatePosition(PositionModel data, int companyId, int userId);
        Task<List<PositionModel>> GetPositionsList(int companyId);
        Task<List<CandCvModel>> GetCandCvsList(int companyId, int cvId, int candidateId);
        Task<List<CandModel?>> GetPosCandsList(int companyId, int positionId, List<int>? candsIds);
        Task DeletePosition(int companyId, int id);
        Task<List<int>> getPositionContactsIds(int companyId, int positionId);
        Task<PositionModel> GetPosition(int companyId, int positionId);
        Task<List<ParserRulesModel>> GetParsersRules(int companyId);
        Task<CvModel?> GetCv(int cvId, int companyId);
        Task UpdateCvKeyId(ImportCvModel importCv);
        Task<List<cv>> CheckIsCvDuplicate(int companyId, int candidateId, int cvAsciiSum);
        Task UpdateCandidateLastCv(ImportCvModel importCv);
        Task UpdateSameCv(ImportCvModel importCv);
        Task<CandPosModel> AttachPosCandCv(AttachePosCandCvModel posCv);
        Task<CandPosModel> DetachPosCand(AttachePosCandCvModel posCv);
        Task<List<company_cvs_email>> GetCompaniesEmails();
        Task<List<int>> SearchCands(int companyId, string searchKeyWords, bool isProximitySearch);
        Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId, List<int>? candsIds);
        Task ActivatePosition( int companyId, PositionModel data);
        Task DactivatePosition(int companyId, PositionModel data);
        Task UpdateCvsAsciiSum(int companyId);
        Task<List<companyStagesTypesModel>> GetCompanyStagesTypes(int companyId);
        Task<bool> SendEmailToCand(EmailToCandModel emailToCand);
        Task<bool> SaveCandReview(int companyId,CandReviewModel candReview);
        Task SaveCandidateToIndex(int companyId, int candidateId);
        Task AddUpdateEmailTemplate(EmailTemplateModel emailTemplate);
        Task DeleteEmailTemplate(int companyId, int id);
        Task<List<EmailTemplateModel>> GetEmailTemplates(int companyId);
        Task UpdateCandDetails(CandDetailsModel candDetails);
        Task SendEmail(SendEmailModel emailData);
    }
}
