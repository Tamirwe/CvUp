using Database.models;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DataModelsLibrary.Queries
{
    public partial class CandsPositionsQueries : ICandsPositionsQueries
    {

        public async Task<List<SearchModel>> GetSearches(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from s in dbContext.searches
                             where s.company_id == companyId
                             orderby s.search_date descending
                             select new SearchModel
                             {
                                 id = s.id,
                                 value = s.val,
                                 advancedValue = s.advanced_val,
                                 exact = s.is_exact,
                                 star = s.is_starred,
                                 updated = s.search_date
                             }).Take(300); ;

                var searchesList = await query.ToListAsync();

                return searchesList;
            }
        }

        public async Task SaveSearch(int companyId, SearchModel searchVals)
        {
            using (var dbContext = new cvup00001Context())
            {
                if (!string.IsNullOrEmpty(searchVals.value))
                {
                    var existSearch = await FindSearchByVals(companyId, searchVals);

                    if (existSearch != null)
                    {
                        existSearch.is_exact = searchVals.exact;
                        existSearch.search_date = DateTime.Now;
                        dbContext.searches.Update(existSearch);
                    }
                    else
                    {
                        dbContext.searches.Add(new search
                        {
                            company_id = companyId,
                            val = searchVals.value,
                            advanced_val = searchVals.advancedValue,
                            is_exact = searchVals.exact,
                            search_date = DateTime.Now
                        });
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteSearch(int companyId, SearchModel searchVals)
        {
            using (var dbContext = new cvup00001Context())
            {
                search? existSearch = null;

                if (searchVals.id != null)
                {
                    existSearch = await dbContext.searches.Where(x => x.company_id == companyId && x.id == searchVals.id).FirstOrDefaultAsync();
                }
                else
                {
                    existSearch = await FindSearchByVals(companyId, searchVals);
                }

                if (existSearch != null)
                {
                    dbContext.searches.Remove(existSearch);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteAllNotStarSearches(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var searchesToDelete = await dbContext.searches.Where(x => x.company_id == companyId && x.is_starred == false).ToListAsync();
                dbContext.searches.RemoveRange(searchesToDelete);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task StarSearch(int companyId, SearchModel searchVals)
        {
            using (var dbContext = new cvup00001Context())
            {
                search? existSearch = null;

                if (searchVals.id != null)
                {
                    existSearch = await dbContext.searches.Where(x => x.company_id == companyId && x.id == searchVals.id).FirstOrDefaultAsync();
                }
                else
                {
                    existSearch = await FindSearchByVals(companyId, searchVals);
                }

                if (existSearch != null)
                {
                    existSearch.is_exact = searchVals.exact;
                    existSearch.is_starred = !existSearch.is_starred;
                    existSearch.search_date = DateTime.Now;
                    dbContext.searches.Update(existSearch);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task<search?> FindSearchByVals(int companyId, SearchModel searchVals)
        {
            using (var dbContext = new cvup00001Context())
            {
                var sVal = string.IsNullOrEmpty(searchVals.value) ? "" : searchVals.value.Trim();

                if (sVal != null)
                {
                    var sAdv = string.IsNullOrEmpty(searchVals.advancedValue) ? null : searchVals.advancedValue.Trim();
                    return await dbContext.searches.Where(x => x.company_id == companyId && x.val == sVal && x.advanced_val == sAdv).FirstOrDefaultAsync();
                }

                return null;
            }
        }

        public async Task<List<keywordsGroupModel>> GetKeywordsGroups(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from s in dbContext.keywords_groups
                             where s.company_id == companyId
                             orderby s.name
                             select new keywordsGroupModel
                             {
                                 id = s.id,
                                 name = s.name,
                             });

                var lst = await query.ToListAsync();

                return lst;
            }
        }

        public async Task SaveKeywordsGroup(int companyId, keywordsGroupModel keywordsGroup)
        {
            using (var dbContext = new cvup00001Context())
            {
                if (keywordsGroup.id != null)
                {
                    var rec = await dbContext.keywords_groups.Where(x => x.company_id == companyId && x.id == keywordsGroup.id).FirstAsync();
                    rec.name = keywordsGroup.name;
                    rec.updated = DateTime.Now;
                    dbContext.keywords_groups.Update(rec);
                }
                else
                {
                    dbContext.keywords_groups.Add(new keywords_group
                    {
                        company_id = companyId,
                        name = keywordsGroup.name,
                    });
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteKeywordsGroup(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                var rec = await dbContext.keywords_groups.Where(x => x.company_id == companyId && x.id == id).FirstAsync();
                dbContext.keywords_groups.Remove(rec);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<keywordModel>> GetKeywords(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from s in dbContext.keywords
                             where s.company_id == companyId
                             orderby s.group_id
                             select new keywordModel
                             {
                                 id = s.id,
                                 nameHe = s.name_he,
                                 nameEn = s.name_en,
                                 groupId = s.group_id,
                                 updated = s.updated,
                             });

                var lst = await query.ToListAsync();

                return lst;
            }
        }

        public async Task SaveKeyword(int companyId, keywordModel keyword)
        {
            using (var dbContext = new cvup00001Context())
            {
                if (keyword.id != null)
                {
                    var rec = await dbContext.keywords.Where(x => x.company_id == companyId && x.id == keyword.id).FirstAsync();
                    rec.name_he = keyword.nameHe;
                    rec.name_en = keyword.nameEn;
                    rec.group_id = keyword.groupId;
                    rec.updated = DateTime.Now;
                    dbContext.keywords.Update(rec);
                }
                else
                {
                    dbContext.keywords.Add(new keyword
                    {
                        company_id = companyId,
                        name_he = keyword.nameHe,
                        name_en = keyword.nameEn,
                        group_id = keyword.groupId,
                    });
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteKeyword(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                var rec = await dbContext.keywords.Where(x => x.company_id == companyId && x.id == id).FirstAsync();
                dbContext.keywords.Remove(rec);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
