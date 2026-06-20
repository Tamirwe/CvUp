using AuthLibrary;
using CandsPositionsLibrary;
using CvFilesLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BlackCandsController : ControllerBase
    {

            private IAuthServise _authServise;

            public BlackCandsController(IAuthServise authServise)
            {
                _authServise = authServise;
            }

           





        }
    }
