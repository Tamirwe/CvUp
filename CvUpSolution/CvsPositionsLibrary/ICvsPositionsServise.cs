﻿using Database.models;
using DataModelsLibrary.Models;

namespace CvsPositionsLibrary
{
    public interface ICvsPositionsServise
    {
        public int AddCv(ImportCvModel importCv);
        public void AddNewCvToIndex(ImportCvModel item);
        public void AddUpdateCandidateFromCvImport(ImportCvModel importCv);
        public void IndexCompanyCvs(int companyId);
        public List<CvListItemModel> GetCvsList(int companyId, int page, int take, int positionId, string? searchKeyWords);
        public department AddDepartment(IdNameModel data, int companyId);
        public department? UpdateDepartment(IdNameModel data, int companyId);
        public List<IdNameModel> GetDepartmentsList(int companyId);
        public void DeleteDepartment(int companyId, int id);
        public hr_company AddHrCompany(IdNameModel data, int companyId);
        public hr_company? UpdateHrCompany(IdNameModel data, int companyId);
        public List<IdNameModel> GetHrCompaniesList(int companyId);
        public void DeleteHrCompany(int companyId, int id);
        public position? AddPosition(PositionClientModel data, int companyId, int userId);
        public position? UpdatePosition(PositionClientModel data, int companyId, int userId);
        public List<PositionListItemModel> GetPositionsList(int companyId);
        public List<CvListItemModel> GetDuplicatesCvsList(int companyId, int cvId, int candidateId);
        public void DeletePosition(int companyId, int id);
        public PositionClientModel GetPosition(int companyId, int positionId);
        public List<ParserRulesModel> GetParsersRules(int companyId);
        public CvModel? GetCv(int cvId, int companyId);
        public void UpdateCvKeyId(ImportCvModel importCv);
        public void SaveCvReview(CvReviewModel cvReview);
        public List<cv> CheckIsCvDuplicate(int companyId, int candidateId,  int cvAsciiSum);
        public void UpdateCandidateLastCv(ImportCvModel importCv);
        public void UpdateSameCv(ImportCvModel importCv);
    }
}