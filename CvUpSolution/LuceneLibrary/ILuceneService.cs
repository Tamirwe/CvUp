

using DataModelsLibrary.Models;

namespace LuceneLibrary
{
    public interface ILuceneService
    {
        Task AddUpdateCandidateDataToIndex(CvsToIndexModel candidateDataToIndex);
        public Task<List<SearchEntry>> Search(int companyId, searchCandCvModel searchVals);
        Task IndexAllCandidates(int companyId, List<CvsToIndexModel> allCandsTextToIndexList);
        //public Task CompanyIndexAddDocuments(int companyId, List<CvsToIndexModel> cvPropsToIndexList,bool isDeleteAllDocuments);
    }
}