

using DataModelsLibrary.Models;

namespace LuceneLibrary
{
    public interface ILuceneService
    {
        public Task DocumentUpdate(int companyId, List<CvsToIndexModel> cvPropsToIndex);
        public Task<List<SearchEntry>> Search(int companyId, string searchQuery);
        public Task CompanyIndexAddDocuments(int companyId, List<CvsToIndexModel> cvPropsToIndexList,bool isDeleteAllDocuments);
    }
}