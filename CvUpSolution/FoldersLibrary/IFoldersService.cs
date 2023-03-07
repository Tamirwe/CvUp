﻿using Database.models;
using DataModelsLibrary.Models;

namespace FoldersLibrary
{
    public interface IFoldersService
    {
        Task<folder> AddFolder( int companyId, FolderModel data);
        Task DeleteFolder(int companyId, int id);
        Task<List<FolderModel>> GetFolders(int companyId);
        Task<folder> UpdateFolder(int companyId, FolderModel data);
    }
}
