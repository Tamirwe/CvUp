using Database.models;
using DataModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{
    public class FoldersQueries : IFoldersQueries
    {
        public async Task<folder> AddFolder(FolderModel data)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteFolder(int companyId, int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FolderModel>> GetFolders(int companyId, int id)
        {
            throw new NotImplementedException();
        }
    }
}
