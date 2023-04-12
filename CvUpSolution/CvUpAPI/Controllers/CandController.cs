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
            return await _candPosService.GetFolderCandsList(Globals.CompanyId, folderId);
        }

        [HttpGet]
        [Route("SearchCands")]
        public async Task<List<CandModel?>> SearchCands(string searchKeyWords = "")
        {
            var candsIds = await _candPosService.SearchCands(Globals.CompanyId, searchKeyWords);
            return await _candPosService.GetCandsList(Globals.CompanyId, 1, 50, candsIds);
        }

        [HttpGet]
        [Route("GetCandCvsList")]
        public async Task<List<CandCvModel>> GetCandCvsList(int cvId, int candidateId)
        {
            return await _candPosService.GetCandCvsList(Globals.CompanyId, cvId, candidateId);
        }

        [HttpGet]
        [Route("GetPosCandsList")]
        public async Task<List<CandModel>> GetPosCandsList(int positionId)
        {
            return await _candPosService.GetPosCandsList(Globals.CompanyId, positionId);
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
    }
}
