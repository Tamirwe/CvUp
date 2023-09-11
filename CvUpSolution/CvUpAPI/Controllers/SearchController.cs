using LuceneLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private ILuceneService _luceneService;
        public SearchController(ILuceneService luceneService)
        {
            _luceneService = luceneService;
        }

        // GET: api/<EmailController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            //_luceneService.BuildIndex();
            //_luceneService.WarmupSearch();
            //_luceneService.Search("הסכם");

            return new string[] { "value1", "value2" };
        }
    }
}
