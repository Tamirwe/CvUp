﻿using Database.models;
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
        public Task<int> AddCv(ImportCvModel importCv);
        public Task AddCvToIndex(ImportCvModel item);
        public Task AddUpdateCandidateFromCvImport(ImportCvModel importCv);
        public Task IndexCompanyCvs(int companyId);
        public Task<List<CandModel?>> GetCandsList(int companyId, int page, int take, List<int>? candsIds);
        public Task<position?> AddPosition(PositionClientModel data, int companyId, int userId);
        public Task<position?> UpdatePosition(PositionClientModel data, int companyId, int userId);
        public Task<List<PositionModel>> GetPositionsList(int companyId);
        public Task<List<CandCvModel>> GetCandCvsList(int companyId, int cvId, int candidateId);
        public Task<List<CandModel>> GetPosCandsList(int companyId, int positionId);
        public Task DeletePosition(int companyId, int id);
        public Task<PositionClientModel> GetPosition(int companyId, int positionId);
        public Task<List<ParserRulesModel>> GetParsersRules(int companyId);
        public Task<CvModel?> GetCv(int cvId, int companyId);
        public Task UpdateCvKeyId(ImportCvModel importCv);
        public Task SaveCvReview(CvReviewModel cvReview);
        public Task<List<cv>> CheckIsCvDuplicate(int companyId, int candidateId, int cvAsciiSum);
        public Task UpdateCandidateLastCv(ImportCvModel importCv);
        public Task UpdateSameCv(ImportCvModel importCv);
        public Task<CandPosModel> AttachPosCandCv(AttachePosCandCvModel posCv);
        public Task<CandPosModel> DetachPosCand(AttachePosCandCvModel posCv);
        public Task<List<company_cvs_email>> GetCompaniesEmails();
        public Task<List<int>> SearchCands(int companyId, string searchKeyWords);
        Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId);
    }
}
