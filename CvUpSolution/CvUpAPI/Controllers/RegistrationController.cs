using CvUpAPI.Services;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicesLibrary.RegisterCompanyAndUser;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private IRegisterCompanyAndUserServise _registerCompanyAndUserServise;
        public RegistrationController(IRegisterCompanyAndUserServise registerCompanyAndUserServise)
        {
            _registerCompanyAndUserServise = registerCompanyAndUserServise;
        }

        [HttpPost]
        public void Post(CompanyAndUserRegisetModel data)
        {
            _registerCompanyAndUserServise.Register(data);
        }
    }
}
