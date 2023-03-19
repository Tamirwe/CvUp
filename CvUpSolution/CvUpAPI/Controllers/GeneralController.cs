using CandsPositionsLibrary;
using Database.models;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private IConfiguration _configuration;
        private ICandsPositionsServise _cvsPosService;

        public GeneralController(IConfiguration config, ICandsPositionsServise cvsPosService)
        {
            _configuration = config;
            _cvsPosService = cvsPosService;
        }

       

    }
}