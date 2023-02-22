using Database.models;
using DataModelsLibrary.Models;
using FoldersLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private IConfiguration _configuration;
        private IFoldersService _foldersService;

        public FoldersController(IConfiguration config, IFoldersService foldersService)
        {
            _configuration = config;
            _foldersService = foldersService;
        }

        [HttpGet]
        [Route("GetFolders")]
        public async Task<IActionResult> GetFolders(int id)
        {
            List<FolderModel> folders = await _foldersService.GetFolders(Globals.CompanyId, id);
            return Ok(folders);
        }

        [HttpPost]
        [Route("AddFolder")]
        public async Task<IActionResult> AddFolder(FolderModel data)
        {
            folder newFolder = await _foldersService.AddFolder( Globals.CompanyId, data);
            return Ok(newFolder);
        }

        [HttpDelete]
        [Route("DeleteFolder")]
        public async Task<IActionResult> DeleteFolder(int id)
        {
            await _foldersService.DeleteFolder(Globals.CompanyId, id);
            return Ok();
        }
    }
}
