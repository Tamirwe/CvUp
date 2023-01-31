using AuthLibrary;
using CvsPositionsLibrary;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CvsController : ControllerBase
    {
        private IConfiguration _configuration;
        private ICvsPositionsServise _cvsPosService;

        public CvsController(IConfiguration config, ICvsPositionsServise cvsPosService)
        {
            _configuration = config;
            _cvsPosService = cvsPosService;
        }

        [HttpGet]
        [Route("GetCvsList")]
        public List<CvListItemModel> GetCvsList(int page = 1, int take = 50, int positionId = 0, string? searchKeyWords = "")
        {
            return _cvsPosService.GetCvsList(Globals.CompanyId, page ,  take , positionId ,  searchKeyWords);
        }

        [HttpGet]
        [Route("GetDuplicatesCvsList")]
        public List<CvListItemModel> GetDuplicatesCvsList(int cvId, int candidateId)
        {
            return _cvsPosService.GetDuplicatesCvsList(Globals.CompanyId, cvId, candidateId);
        }

        [HttpGet]
        [Route("GetPosCvsList")]
        public List<CvListItemModel> GetPosCvsList(int posId)
        {
            return _cvsPosService.GetPosCvsList(Globals.CompanyId, posId);
        }

        [HttpGet]
        [Route("getCv")]
        public CvModel? getCv(int cvId)
        {
            return _cvsPosService.GetCv(cvId, Globals.CompanyId);
        }

        [HttpPost]
        [Route("SaveCvReview")]
        public void SaveCvReview(CvReviewModel cvReview)
        {
            _cvsPosService.SaveCvReview(cvReview);
        }

        [HttpPost]
        [Route("AttachePosCv")]
        public void AttachePosCv(AttachePosCvModel posCv)
        {
            posCv.companyId= Globals.CompanyId;
            _cvsPosService.AttachePosCv(posCv);
        }
    }
}
