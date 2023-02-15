

using DataModelsLibrary.Models;

namespace LuceneLibrary
{
    public interface ILuceneService
    {
        public Task DocumentAdd(int companyId, CvPropsToIndexModel cvPropsToIndex);
        public Task DocumentUpdate(int companyId, CvPropsToIndexModel cvPropsToIndex);
        public Task<List<SearchEntry>> Search(int companyId, string searchQuery);
        public Task BuildCompanyIndex(int companyId, List<CvPropsToIndexModel> cvPropsToIndexList);
    }
}