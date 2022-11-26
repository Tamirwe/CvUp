

using DataModelsLibrary.Models;

namespace LuceneLibrary
{
    public interface ILuceneService
    {
        //public void BuildIndex();
        //public void DocumentAdd();
        public void DocumentDelete();
        public void WarmupSearch(int companyId);
        public IEnumerable<SearchEntry> Search(int companyId, string searchQuery);
        public void BuildCompanyIndex(int companyId, List<CompanyTextToIndexModel> CompanyTextToIndexList);
    }
}