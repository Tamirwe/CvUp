﻿using CandsPositionsLibrary;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public List<CandModel> GetCandsList(int page = 1, int take = 50, int positionId = 0, string? searchKeyWords = "")
        {
            return _candPosService.GetCandsList(Globals.CompanyId, page, take, positionId, searchKeyWords);
        }

        [HttpGet]
        [Route("GetCandCvsList")]
        public List<CandModel> GetCandCvsList(int cvId, int candidateId)
        {
            return _candPosService.GetCandCvsList(Globals.CompanyId, cvId, candidateId);
        }

        [HttpGet]
        [Route("GetPosCandsList")]
        public List<CandModel> GetPosCandsList(int positionId)
        {
            return _candPosService.GetPosCandsList(Globals.CompanyId, positionId);
        }

        [HttpGet]
        [Route("getCv")]
        public CvModel? getCv(int cvId)
        {
            return _candPosService.GetCv(cvId, Globals.CompanyId);
        }

        [HttpPost]
        [Route("SaveCvReview")]
        public void SaveCvReview(CvReviewModel cvReview)
        {
            _candPosService.SaveCvReview(cvReview);
        }

        [HttpPost]
        [Route("AttachPosCandCv")]
        public CandPosModel AttachPosCandCv(AttachePosCandCvModel posCv)
        {
            posCv.companyId = Globals.CompanyId;
            return _candPosService.AttachPosCandCv(posCv);
        }

        [HttpPost]
        [Route("DetachPosCand")]
        public CandPosModel DetachPosCand(AttachePosCandCvModel posCv)
        {
            posCv.companyId = Globals.CompanyId;
            return _candPosService.DetachPosCand(posCv);
        }
    }
}