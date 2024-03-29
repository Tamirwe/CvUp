﻿using AuthLibrary;
using CandsPositionsLibrary;
using CvFilesLibrary;
using Database.models;
using DataModelsLibrary.Models;
using EmailsLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Org.BouncyCastle.Asn1.Cmp;
using System.ComponentModel.Design;


namespace CvUpAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CandController : ControllerBase
    {
        private ICandsPositionsServise _candPosService;
        private IAuthServise _authServise;
        private ICvsFilesService _cvsFilesService;

        public CandController(ICandsPositionsServise candPosService, IAuthServise authServise, ICvsFilesService cvsFilesService)
        {
            _candPosService = candPosService;
            _authServise = authServise;
            _cvsFilesService = cvsFilesService;
        }

        [HttpGet]
        [Route("GetIsAuthorized")]
        public IActionResult GetIsAuthorized()
        {
            return Ok();
        }

        [HttpGet]
        [Route("GetCandsList")]
        public async Task<List<CandModel?>> GetCandsList(int page = 1, int take = 200)
        {
            return await _candPosService.GetCandsList(Globals.CompanyId, null);
        }

        [HttpGet]
        [Route("GetFolderCandsList")]
        public async Task<List<CandModel?>> GetFolderCandsList(int folderId)
        {
            return await _candPosService.GetFolderCandsList(Globals.CompanyId, folderId);
        }

        [HttpGet]
        [Route("GetPosTypeCandsList")]
        public async Task<List<CandModel?>> GetPosTypeCandsList(int positionTypeId)
        {
            return await _candPosService.GetPosTypeCandsList(Globals.CompanyId, positionTypeId);
        }

        [HttpPost]
        [Route("SearchCands")]
        public async Task<IEnumerable<CandModel?>> SearchCands(searchCandCvModel searchVals)
        {
            List<CandModel?> candsList = new List<CandModel?>();
            var results = await _candPosService.SearchCands(Globals.CompanyId, searchVals);

            var candsIds = results.Select(e => e.Id).ToList();

            if (searchVals.folderId > 0)
            {
                candsList = await _candPosService.GetFolderCandsList(Globals.CompanyId, searchVals.folderId);
                candsList = candsList.Where(x => candsIds.Any(y => y == x.candidateId)).ToList();
            }
            else if (searchVals.positionId > 0)
            {
                candsList = await _candPosService.GetPosCandsList(Globals.CompanyId, searchVals.positionId);
                candsList = candsList.Where(x => candsIds.Any(y => y == x.candidateId)).ToList();
            }
            else if (searchVals.positionTypeId > 0)
            {
                candsList = await _candPosService.GetPosTypeCandsList(Globals.CompanyId, searchVals.positionTypeId);
                candsList = candsList.Where(x => candsIds.Any(y => y == x.candidateId)).ToList();
            }
            else
            {
                var firstRows = candsIds.GetRange(0, candsIds.Count > 300 ? 300: candsIds.Count);
                candsList = await _candPosService.GetCandsList(Globals.CompanyId, firstRows);
            }

            foreach (var res in results)
            {
                var itemToChange = candsList.FirstOrDefault(x => x.candidateId == res.Id);

                if (itemToChange != null)
                {
                    itemToChange.score = res.Score;
                }
            }

            var sortedCands = candsList.OrderByDescending(x => x.score).ToList();
            return sortedCands;
        }

        [HttpGet]
        [Route("GetCandCvsList")]
        public async Task<List<CandCvModel>> GetCandCvsList(int candidateId)
        {
            return await _candPosService.GetCandCvsList(Globals.CompanyId, candidateId);
        }

        [HttpGet]
        [Route("GetPosCandsList")]
        public async Task<List<CandModel?>> GetPosCandsList(int positionId)
        {
            return await _candPosService.GetPosCandsList(Globals.CompanyId, positionId);
        }

        [HttpGet]
        [Route("getCv")]
        public async Task<CvModel?> getCv(int cvId)
        {
            return await _candPosService.GetCv(cvId, Globals.CompanyId);
        }

        [HttpPut]
        [Route("SaveCandReview")]
        public async Task<CandModel?> SaveCandReview(CandReviewModel candReview)
        {
            await _candPosService.SaveCandReview(Globals.CompanyId, candReview);
            return await _candPosService.GetCandidate(Globals.CompanyId, candReview.candidateId);
        }

        [HttpPost]
        [Route("AttachPosCandCv")]
        public async Task<CandModel?> AttachPosCandCv(AttachePosCandCvModel posCv)
        {
            posCv.companyId = Globals.CompanyId;
            await _candPosService.AttachPosCandCv(posCv);
            await _candPosService.UpdatePositionDate(posCv.companyId, posCv.positionId, true);
            return await _candPosService.GetCandidate(posCv.companyId, posCv.candidateId);

        }

        [HttpPost]
        [Route("DetachPosCand")]
        public async Task<CandModel?> DetachPosCand(AttachePosCandCvModel posCv)
        {
            posCv.companyId = Globals.CompanyId;
            await _candPosService.DetachPosCand(posCv);
            await _candPosService.UpdatePositionDate(posCv.companyId, posCv.positionId, true);
            return await _candPosService.GetCandidate(posCv.companyId, posCv.candidateId);
        }

        [HttpGet]
        [Route("GetCandPosStagesTypes")]
        public async Task<List<CandPosStageTypeModel>> GetCandPosStagesTypes()
        {
            return await _candPosService.GetCandPosStagesTypes(Globals.CompanyId);
        }

        [HttpPost]
        [Route("SendEmail")]
        public async Task<IActionResult> SendEmail(SendEmailModel emailData)
        {
            emailData.companyId = Globals.CompanyId;
            UserModel? user = await _authServise.GetUser(Globals.CompanyId, Globals.UserId);
            await _candPosService.SendEmail(emailData, user);
            await _candPosService.AddSendEmail(emailData, Globals.UserId);
            return Ok();
        }

        [HttpPost]
        [Route("UpdateCandPositionStatus")]
        public async Task<CandModel?> UpdateCandPositionStatus(CandPosStageTypeUpdateModel posStatus)
        {
            posStatus.companyId = Globals.CompanyId;
            await _candPosService.UpdateCandPositionStatus(posStatus);
            await _candPosService.UpdatePositionDate(posStatus.companyId, posStatus.positionId, false);
            return await _candPosService.GetPositionCandidate(posStatus.companyId, posStatus.candidateId, posStatus.positionId);
        }

        [HttpPost]
        [Route("UpdatePosStageDate")]
        public async Task<CandModel?> UpdatePosStageDate(CandPosStageTypeUpdateModel posStatus)
        {
            posStatus.companyId = Globals.CompanyId;
            await _candPosService.UpdatePosStageDate(posStatus);
            await _candPosService.UpdatePositionDate(posStatus.companyId, posStatus.positionId, false);
            return await _candPosService.GetPositionCandidate(posStatus.companyId, posStatus.candidateId, posStatus.positionId);
        }

        [HttpPost]
        [Route("RemovePosStage")]
        public async Task<CandModel?> RemovePosStage(CandPosStageTypeUpdateModel posStatus)
        {
            posStatus.companyId = Globals.CompanyId;
            await _candPosService.RemovePosStage(posStatus);
            await _candPosService.UpdatePositionDate(posStatus.companyId, posStatus.positionId, false);
            return await _candPosService.GetPositionCandidate(posStatus.companyId, posStatus.candidateId, posStatus.positionId);
        }

        [HttpGet]
        [Route("GetEmailTemplates")]
        public async Task<List<EmailTemplateModel>> GetEmailTemplates()
        {
            return await _candPosService.GetEmailTemplates(Globals.CompanyId);
        }

        [HttpPost]
        [Route("AddUpdateEmailTemplate")]
        public async Task<IActionResult> AddUpdateEmailTemplate(EmailTemplateModel emailTemplate)
        {
            emailTemplate.companyId = Globals.CompanyId;
            await _candPosService.AddUpdateEmailTemplate(emailTemplate);
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteEmailTemplate")]
        public async Task<IActionResult> DeleteEmailTemplate(int id)
        {
            await _candPosService.DeleteEmailTemplate(Globals.CompanyId, id);
            return Ok();
        }

        [HttpPut]
        [Route("UpdateCandDetails")]
        public async Task<CandModel?> UpdateCandDetails(CandDetailsModel candDetails)
        {
            candDetails.companyId = Globals.CompanyId;
            await _candPosService.UpdateCandDetails(candDetails);
            return await _candPosService.GetCandidate(candDetails.companyId, candDetails.candidateId);
        }

        [HttpGet]
        [Route("UpdateIsSeen")]
        public async Task<IActionResult> UpdateIsSeen(int cvId)
        {
            await _candPosService.UpdateIsSeen(Globals.CompanyId, cvId);
            return Ok();
        }

        [HttpGet]
        [Route("CandsReport")]
        public async Task<List<CandReportModel?>> CandsReport(string stageType)
        {
            return await _candPosService.CandsReport(Globals.CompanyId, stageType);
        }

        [HttpDelete]
        [Route("DeleteCv")]
        public async Task<CandModel?> DeleteCv(int cnid, int cvId)
        {
            await _candPosService.DeleteCv(Globals.CompanyId, cnid, cvId);
            return await _candPosService.GetCandidate(Globals.CompanyId, cnid);

        }

        [HttpDelete]
        [Route("DeleteCandidate")]
        public async Task<IActionResult> DeleteCandidate(int id)
        {
            await _candPosService.DeleteCandidate(Globals.CompanyId, id);
            return Ok();
        }

        [HttpPut]
        [Route("SaveCustomerCandReview")]
        public async Task<CandModel?> SaveCustomerCandReview(CandReviewModel customerCandReview)
        {
            await _candPosService.SaveCustomerCandReview(Globals.CompanyId, customerCandReview);
            return await _candPosService.GetCandidate(Globals.CompanyId, customerCandReview.candidateId);
        }

        [HttpGet]
        [Route("GetSearches")]
        public async Task<List<SearchModel>> GetSearches()
        {
            return await _candPosService.GetSearches(Globals.CompanyId);
        }

        [HttpPost]
        [Route("SaveSearch")]
        public async Task<IActionResult> SaveSearch(SearchModel searchVals)
        {
            await _candPosService.SaveSearch(Globals.CompanyId, searchVals);
            return Ok();
        }
      
        [HttpPut]
        [Route("DeleteSearch")]
        public async Task<IActionResult> DeleteSearch(SearchModel searchVals)
        {
            await _candPosService.DeleteSearch(Globals.CompanyId, searchVals);
            return Ok();
        }

        [HttpPut]
        [Route("StarSearch")]
        public async Task<IActionResult> StarSearch(SearchModel searchVals)
        {
            await _candPosService.StarSearch(Globals.CompanyId, searchVals);
            return Ok();
        }

        [HttpPut]
        [Route("DeleteAllNotStarSearches")]
        public async Task<List<SearchModel>> DeleteAllNotStarSearches()
        {
            await _candPosService.DeleteAllNotStarSearches(Globals.CompanyId);
            return await _candPosService.GetSearches(Globals.CompanyId);
        }

        [HttpGet]
        [Route("GetKeywordsGroups")]
        public async Task<List<keywordsGroupModel>> GetKeywordsGroups()
        {
            return await _candPosService.GetKeywordsGroups(Globals.CompanyId);
        }

        [HttpPost]
        [Route("SaveKeywordsGroup")]
        public async Task<IActionResult> SaveKeywordsGroup(keywordsGroupModel keywordsGroup)
        {
            await _candPosService.SaveKeywordsGroup(Globals.CompanyId, keywordsGroup);
            return Ok();
        }

        [HttpPut]
        [Route("DeleteKeywordsGroup")]
        public async Task<IActionResult> DeleteKeywordsGroup(int id)
        {
            await _candPosService.DeleteKeywordsGroup(Globals.CompanyId, id);
            return Ok();
        }

        [HttpGet]
        [Route("GetKeywords")]
        public async Task<List<keywordModel>> GetKeywords()
        {
            return await _candPosService.GetKeywords(Globals.CompanyId);
        }

        [HttpPost]
        [Route("SaveKeyword")]
        public async Task<IActionResult> SaveKeyword(keywordModel keyword)
        {
            await _candPosService.SaveKeyword(Globals.CompanyId, keyword);
            return Ok();
        }

        [HttpPut]
        [Route("DeleteKeyword")]
        public async Task<IActionResult> DeleteKeyword(int id)
        {
            await _candPosService.DeleteKeyword(Globals.CompanyId, id);
            return Ok();
        }
    }
}
