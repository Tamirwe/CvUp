using CandsPositionsLibrary;
using Database.models;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        public async Task<IEnumerable<CandModel?>> SearchCands(searchCandCvModel search)
        {
            List<CandModel?> candsList;
            var candsIds = await _candPosService.SearchCands(Globals.CompanyId, search.keyWords);

            if (search.folderId > 0)
            {
                candsList = await _candPosService.GetFolderCandsList(Globals.CompanyId, search.folderId, candsIds);
                candsList = candsList.Where(x => candsIds.Any(y => y == x.candidateId)).ToList();
            }
            else if (search.positionId > 0)
            {
                candsList = await _candPosService.GetPosCandsList(Globals.CompanyId, search.positionId, candsIds);
                candsList = candsList.Where(x => candsIds.Any(y => y == x.candidateId)).ToList();
            }
            else
            {
                candsList = await _candPosService.GetCandsList(Globals.CompanyId, 1, 50, candsIds);
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

        [HttpPost]
        [Route("SaveCvReview")]
        public void SaveCvReview(CvReviewModel cvReview)
        {
            _candPosService.SaveCvReview(cvReview);
        }

        [HttpPut]
        [Route("SaveCandReview")]
        public async Task<IActionResult> SaveCandReview(CandReviewModel candReview)
        {
            bool isSaved = await _candPosService.SaveCandReview(candReview);

            if (isSaved) {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("AttachPosCandCv")]
        public async Task<CandPosModel> AttachPosCandCv(AttachePosCandCvModel posCv)
        {
            posCv.companyId = Globals.CompanyId;
            return await _candPosService.AttachPosCandCv(posCv);
        }

        [HttpPost]
        [Route("DetachPosCand")]
        public async Task<CandPosModel> DetachPosCand(AttachePosCandCvModel posCv)
        {
            posCv.companyId = Globals.CompanyId;
            return await _candPosService.DetachPosCand(posCv);
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

    }
}
