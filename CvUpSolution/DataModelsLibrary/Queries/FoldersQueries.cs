using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{
    public class FoldersQueries : IFoldersQueries
    {
        public async Task<folder> AddFolder(int companyId, FolderModel data)
        {
            using (var dbContext = new cvup00001Context())
            {
                var fdr = new folder
                {
                    name = data.name,
                    parent_id = data.parentId,
                    company_id = companyId,
                };

                dbContext.folders.Add(fdr);
                await dbContext.SaveChangesAsync();
                return fdr;
            }
        }

        public async Task<folder> UpdateFolder(int companyId, FolderModel data)
        {
            using (var dbContext = new cvup00001Context())
            {
                var fdr = new folder
                {
                    id = data.id,
                    name = data.name,
                    parent_id = data.parentId,
                    company_id = companyId,
                };

                dbContext.folders.Update(fdr);
                await dbContext.SaveChangesAsync();
                return fdr;
            }
        }


        public async Task DeleteFolder(int companyId, List<int> ids)
        {

            using (var dbContext = new cvup00001Context())
            {
                var fdr = await (from f in dbContext.folders
                                 where ids.Contains(f.id) && f.company_id == companyId
                                 select f).ToListAsync();

                foreach (var item in fdr)
                {
                    dbContext.folders.Remove(item);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<FolderModel>> GetFolders(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from f in dbContext.folders
                            where f.company_id == companyId
                            orderby f.name
                            select new FolderModel
                            {
                                id = f.id,
                                name = f.name,
                                parentId = f.parent_id
                            };

                return await query.ToListAsync();
            }
        }

        public async Task AttachCandidate(int companyId, FolderCandidateModel data)
        {
            using (var dbContext = new cvup00001Context())
            {
                var fdr = new folders_cand
                {
                    company_id = companyId,
                    candidate_id = data.candidateId,
                    folder_id = data.folderId,
                };

                dbContext.folders_cands.Add(fdr);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DetachCandidate(int companyId, FolderCandidateModel data)
        {
            using (var dbContext = new cvup00001Context())
            {
                folders_cand? candFolder = await dbContext.folders_cands.Where(x => x.company_id == companyId
                    && x.folder_id == data.folderId && x.candidate_id == data.candidateId).FirstOrDefaultAsync();

                if (candFolder != null)
                {
                    dbContext.folders_cands.Remove(candFolder);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateCandidateFolders(int companyId, int candidateId)
        {
            using (var dbContext = new cvup00001Context())
            {
                List<folders_cand>? foldersCandList = await dbContext.folders_cands.Where(x => x.company_id == companyId
                   && x.candidate_id == candidateId).ToListAsync();

                List<int> candFoldersIds = new List<int>();

                foreach (var item in foldersCandList)
                {
                    candFoldersIds.Add(item.folder_id);
                }


                candidate? cand = dbContext.candidates.Where(x => x.company_id == companyId && x.id == candidateId).FirstOrDefault();

                if (cand != null)
                {
                    cand.folders_ids = $"[{string.Join(",", candFoldersIds)}]";
                    var result = dbContext.candidates.Update(cand);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
       
    }
}
