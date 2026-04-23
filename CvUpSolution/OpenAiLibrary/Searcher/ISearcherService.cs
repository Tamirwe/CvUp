using DataModelsLibrary.Models;
using OpenAiLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiLibrary.Searcher
{
    public interface ISearcherService
    {
        Task<List<SearchResultModel>> SearchAsync(string query, SearchFilterModel filter , int limit = 10);
        Task DemoSearch();
    }
}
