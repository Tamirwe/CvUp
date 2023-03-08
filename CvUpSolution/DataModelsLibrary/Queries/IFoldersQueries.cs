using Database.models;
using DataModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{
    public interface IFoldersQueries
    {
        Task<folder> AddFolder(int companyId, FolderModel data);
        Task<folders_cand> AttachCandidate(int companyId, FolderCandidateModel data);
        Task<folders_cand> DetachCandidate(int companyId, int id);
        Task DeleteFolder(int companyId, List<int> ids);
        Task<List<FolderModel>> GetFolders(int companyId);
        Task<folder> UpdateFolder(int companyId, FolderModel data);
    }
}
