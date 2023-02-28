using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Microsoft.Extensions.Configuration;

namespace FoldersLibrary
{
    public class FoldersService : IFoldersService
    {
        private IConfiguration _configuration;
        private IFoldersQueries _foldersQueries;

        public FoldersService(IConfiguration config, IFoldersQueries foldersQueries)
        {
            _configuration = config;
            _foldersQueries = foldersQueries;
        }

        public async Task<folder> AddFolder(int companyId, FolderModel data)
        {
            return await _foldersQueries.AddFolder(companyId,data);
        }

        public async Task DeleteFolder(int companyId, int id)
        {
            await _foldersQueries.DeleteFolder(companyId, id);
        }

        public async Task<List<FolderModel>> GetFolders(int companyId)
        {
            return await _foldersQueries.GetFolders(companyId);
        }
    }
}
