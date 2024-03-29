﻿using Database.models;
using DataModelsLibrary.Models;
using LuceneLibrary;
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
        Task DeleteCv(int companyId, int candidateId, int cvId);
        Task DeleteCandidate(int companyId, int candidateId);
        Task AddUpdateCandidateFromCvImport(ImportCvModel importCv);
        Task IndexCompanyCvs(int companyId);
        Task<List<CandModel?>> GetCandsList(int companyId, List<int>? candsIds);
        Task<int> AddPosition(PositionModel data, int companyId, int userId);
        Task<int> UpdatePosition(PositionModel data, int companyId, int userId);
        Task<List<PositionModel>> GetPositionsList(int companyId);
        Task<CandModel?> GetCandidate(int companyId, int candId);
        Task<CandModel?> GetPositionCandidate(int companyId, int candId, int positionId);
        Task<List<CandCvModel>> GetCandCvsList(int companyId,  int candidateId);
        Task<List<CandModel?>> GetPosCandsList(int companyId, int positionId);
        Task<List<CandModel?>> GetPosTypeCandsList(int companyId, int positionTypeId);
        Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId);
        Task DeletePosition(int companyId, int id);
        Task<List<int>> getPositionContactsIds(int companyId, int positionId);
        Task<PositionModel> GetPosition(int companyId, int positionId);
        Task<List<ParserRulesModel>> GetParsersRules();
        Task<CvModel?> GetCv(int cvId, int companyId);
        Task UpdateCvKeyId(ImportCvModel importCv);
        Task<cvs_txt?> CheckIsSameCv(int companyId, int candidateId, int cvAsciiSum);
        Task UpdateCandLastCv(int companyId, int candidateId, int cvId, bool isDuplicate, DateTime lastCvSent);
        Task UpdateCvDate(int cvId);
        Task AttachPosCandCv(AttachePosCandCvModel posCv);
        Task DetachPosCand(AttachePosCandCvModel posCv);
        Task<List<company_cvs_email>> GetCompaniesEmails();
        Task<List<SearchEntry>> SearchCands(int companyId, searchCandCvModel searchVals);
        Task UpdateCvsAsciiSum(int companyId);
        Task<List<CandPosStageTypeModel>> GetCandPosStagesTypes(int companyId);
        Task SaveCandReview(int companyId,CandReviewModel candReview);
        Task SaveCandidateToIndex(int companyId, int candidateId);
        Task AddUpdateEmailTemplate(EmailTemplateModel emailTemplate);
        Task DeleteEmailTemplate(int companyId, int id);
        Task<List<EmailTemplateModel>> GetEmailTemplates(int companyId);
        Task UpdateCandDetails(CandDetailsModel candDetails);
        Task SendEmail(SendEmailModel emailData, UserModel? user);
        Task UpdateCandPositionStatus(CandPosStageTypeUpdateModel posStatus);
        Task UpdatePosStageDate(CandPosStageTypeUpdateModel posStatus);
        Task RemovePosStage(CandPosStageTypeUpdateModel posStatus);
        Task UpdateIsSeen(int companyId, int cvId);
        Task<List<CandReportModel?>> CandsReport(int companyId, string stageType);
        Task UpdatePositionDate(int companyId, int positionId,bool isUpdateCount);
        Task<position?> GetPositionByMatchStr(int companyId, string matchStr);
        Task AddSendEmail(SendEmailModel emailData, int userId);
        Task<int?> GetPositionTypeId(int companyId, string positionRelated);
        Task<int> AddPositionTypeName(int companyId, string positionRelated);
        Task<List<PositionTypeModel>> GetPositionsTypes(int companyId);
        Task SaveCustomerCandReview(int companyId, CandReviewModel customerCandReview);
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
    }
}
