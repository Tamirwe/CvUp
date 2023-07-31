using CandsPositionsLibrary;
using Database.models;
using DataModelsLibrary.Models;
using EmailsLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Asn1.Cmp;
using System.ComponentModel.Design;

namespace CvUpAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CandController : ControllerBase
    {
        private IConfiguration _configuration;
        private ICandsPositionsServise _candPosService;

        public CandController(IConfiguration config, ICandsPositionsServise candPosService)
        {
            _configuration = config;
            _candPosService = candPosService;
        }

        [HttpGet]
        [Route("GetCandsList")]
        public async Task<List<CandModel?>> GetCandsList(int page = 1, int take = 200)
        {
            return await _candPosService.GetCandsList(Globals.CompanyId, page, take, null);
        }

        [HttpGet]
        [Route("GetFolderCandsList")]
        public async Task<List<CandModel?>> GetFolderCandsList(int folderId)
        {
            return await _candPosService.GetFolderCandsList(Globals.CompanyId, folderId, null);
        }

        [HttpPost]
        [Route("SearchCands")]
        public async Task<IEnumerable<CandModel?>> SearchCands(searchCandCvModel searchVals)
        {
            List<CandModel?> candsList;
            var candsIds = await _candPosService.SearchCands(Globals.CompanyId, searchVals);

            if (searchVals.folderId > 0)
            {
                candsList = await _candPosService.GetFolderCandsList(Globals.CompanyId, searchVals.folderId, candsIds);
                candsList = candsList.Where(x => candsIds.Any(y => y == x.candidateId)).ToList();
            }
            else if (searchVals.positionId > 0)
            {
                candsList = await _candPosService.GetPosCandsList(Globals.CompanyId, searchVals.positionId, candsIds);
                candsList = candsList.Where(x => candsIds.Any(y => y == x.candidateId)).ToList();
            }
            else
            {
                candsList = await _candPosService.GetCandsList(Globals.CompanyId, 1, 300, candsIds);
            }

            var sortedCands = candsList.OrderBy(x => candsIds.IndexOf(x.candidateId)).ToList();
            return sortedCands.Take( 300); 
        }

        [HttpGet]
        [Route("GetCandCvsList")]
        public async Task<List<CandCvModel>> GetCandCvsList(int cvId, int candidateId)
        {
            return await _candPosService.GetCandCvsList(Globals.CompanyId, cvId, candidateId);
        }

        [HttpGet]
        [Route("GetPosCandsList")]
        public async Task<List<CandModel?>> GetPosCandsList(int positionId)
        {
            return await _candPosService.GetPosCandsList(Globals.CompanyId, positionId, null);
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
            return await _candPosService.GetCandidate(posCv.companyId, posCv.candidateId);

        }

        [HttpPost]
        [Route("DetachPosCand")]
        public async Task<CandModel?> DetachPosCand(AttachePosCandCvModel posCv)
        {
            posCv.companyId = Globals.CompanyId;
            await _candPosService.DetachPosCand(posCv);
            return await _candPosService.GetCandidate(posCv.companyId, posCv.candidateId);
        }

        [HttpPost]
        [Route("UpdateCandPositionStatus")]
        public async Task<CandModel?> UpdateCandPositionStatus(CandPosStatusUpdateCvModel posStatus)
        {
            posStatus.companyId = Globals.CompanyId;
            await _candPosService.UpdateCandPositionStatus(posStatus);
            return await _candPosService.GetCandidate(posStatus.companyId, posStatus.candidateId);
        }

        [HttpGet]
        [Route("GetCompanyStagesTypes")]
        public async Task<List<companyStagesTypesModel>> GetCompanyStagesTypes()
        {
            return await _candPosService.GetCompanyStagesTypes(Globals.CompanyId);
        }

        [HttpPost]
        [Route("SendEmailToCand")]
        public async Task<bool> SendEmailToCand(EmailToCandModel emailToCand)
        {
            emailToCand.companyId = Globals.CompanyId;
            return await _candPosService.SendEmailToCand(emailToCand);
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

        [HttpPost]
        [Route("SendEmail")]
        public async Task<IActionResult> SendEmail(SendEmailModel emailData)
        {
            emailData.companyId = Globals.CompanyId;
            await _candPosService.SendEmail(emailData);
            return Ok();
        }

        [HttpGet]
        [Route("UpdateIsSeen")]
        public async Task<IActionResult> UpdateIsSeen(int cvId)
        {
            await _candPosService.UpdateIsSeen(Globals.CompanyId, cvId);
            return Ok();
        }
       
    }
}
