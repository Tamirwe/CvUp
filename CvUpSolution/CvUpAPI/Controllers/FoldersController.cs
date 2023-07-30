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
        public async Task<IActionResult> GetFolders()
        {
            List<FolderModel> folders = await _foldersService.GetFolders(Globals.CompanyId);
            return Ok(folders);
        }

        [HttpPost]
        [Route("AddFolder")]
        public async Task<IActionResult> AddFolder(FolderModel data)
        {
            folder newFolder = await _foldersService.AddFolder(Globals.CompanyId, data);
            return Ok(newFolder);
        }

        [HttpPut]
        [Route("UpdateFolder")]
        public async Task<IActionResult> UpdateFolder(FolderModel data)
        {
            folder newFolder = await _foldersService.UpdateFolder(Globals.CompanyId, data);
            return Ok(newFolder);
        }

        [HttpDelete]
        [Route("DeleteFolder")]
        public async Task<IActionResult> DeleteFolder(int id)
        {
            await _foldersService.DeleteFolder(Globals.CompanyId, id);
            return Ok();
        }

        [HttpPost]
        [Route("AttachCandidate")]
        public async Task<IActionResult> AttachCandidate(FolderCandidateModel data)
        {
            await _foldersService.AttachCandidate(Globals.CompanyId, data);
            return Ok();
        }

        [HttpPost]
        [Route("DetachCandidate")]
        public async Task<IActionResult> DetachCandidate(FolderCandidateModel data)
        {
            await _foldersService.DetachCandidate(Globals.CompanyId, data);
            return Ok();
        }
    }
}
