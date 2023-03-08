using Database.models;
using DataModelsLibrary.Models;

namespace FoldersLibrary
{
    public interface IFoldersService
    {
        Task<folder> AddFolder( int companyId, FolderModel data);
        Task<folders_cand> AttachCandidate(int companyId, FolderCandidateModel data);
        Task<folders_cand> DetachCandidate(int companyId, int id);
        Task DeleteFolder(int companyId, int id);
        Task<List<FolderModel>> GetFolders(int companyId);
        Task<folder> UpdateFolder(int companyId, FolderModel data);
    }
}
