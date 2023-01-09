

using DataModelsLibrary.Models;

namespace LuceneLibrary
{
    public interface ILuceneService
    {
        public void DocumentAdd(int companyId, CvPropsToIndexModel cvPropsToIndex);
        public void DocumentUpdate(int companyId, CvPropsToIndexModel cvPropsToIndex);
        public void WarmupSearch(int companyId);
        public IEnumerable<SearchEntry> Search(int companyId, string searchQuery);
        public void BuildCompanyIndex(int companyId, List<CvPropsToIndexModel> cvPropsToIndexList);
    }
}