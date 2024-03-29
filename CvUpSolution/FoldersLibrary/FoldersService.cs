﻿using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.Design;

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
            return await _foldersQueries.AddFolder(companyId, data);
        }

        public async Task<folder> UpdateFolder(int companyId, FolderModel data)
        {
            return await _foldersQueries.UpdateFolder(companyId, data);
        }

        public async Task DeleteFolder(int companyId, int id)
        {
            List<FolderModel> allFolders = await _foldersQueries.GetFolders(companyId);
            List<int> foldersToDelete = new List<int>() { id };
            getFolderChilds(allFolders, id, foldersToDelete);

            await _foldersQueries.DeleteFolder(companyId, foldersToDelete);
        }

        private void getFolderChilds(List<FolderModel> allFolders, int parentId, List<int> foldersToDelete)
        {
            var childs = allFolders.Where(x => x.parentId == parentId).ToList();

            childs.ForEach(x =>
            {
                foldersToDelete.Add(x.id);
                getFolderChilds(allFolders, x.id, foldersToDelete);
            });

            return;
        }

        public async Task<List<FolderModel>> GetFolders(int companyId)
        {
            return await _foldersQueries.GetFolders(companyId);
        }

        public async Task AttachCandidate(int companyId, FolderCandidateModel data)
        {
            await _foldersQueries.AttachCandidate(companyId, data);
            await _foldersQueries.UpdateCandidateFolders(companyId, data.candidateId);
        }

        public async Task DetachCandidate(int companyId, FolderCandidateModel data)
        {
            await _foldersQueries.DetachCandidate(companyId, data);
            await _foldersQueries.UpdateCandidateFolders(companyId, data.candidateId);
        }

    }
}
