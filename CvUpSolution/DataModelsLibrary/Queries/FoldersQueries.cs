using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                    parent_id=data.parentId,
                    company_id=companyId,
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
                    id= data.id,
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
                                parentId=f.parent_id
                            };

                return await query.ToListAsync();
            }
        }
    }
}
