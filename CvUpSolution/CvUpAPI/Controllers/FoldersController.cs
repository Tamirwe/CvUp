using CandsPositionsLibrary;
using Database.models;
using DataModelsLibrary.Models;
using FoldersLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private ICandsServise _candsService;
        private IFoldersService _foldersService;

        public FoldersController(ICandsServise candsService, IFoldersService foldersService)
        {
            _candsService = candsService;
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
        public async Task<CandModel?> AttachCandidate(FolderCandidateModel data)
        {
            await _foldersService.AttachCandidate(Globals.CompanyId, data);
            return await _candsService.GetCandidate(Globals.CompanyId, data.candidateId);
        }

        [HttpPost]
        [Route("DetachCandidate")]
        public async Task<CandModel?> DetachCandidate(FolderCandidateModel data)
        {
            await _foldersService.DetachCandidate(Globals.CompanyId, data);
            return await _candsService.GetCandidate(Globals.CompanyId, data.candidateId);
        }
    }
}
