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

            private ICandsPositionsServise _candPosService;
            private IAuthServise _authServise;

            public BlackCandsController(ICandsPositionsServise candPosService, IAuthServise authServise)
            {
                _candPosService = candPosService;
                _authServise = authServise;
            }

           





        }
    }
