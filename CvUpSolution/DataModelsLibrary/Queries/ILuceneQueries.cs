using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface ILuceneQueries
    {
        Task<List<CvsToIndexModel>> GetCandidatesLastCvsToIndex(int companyId, int candidateId = 0);
    }
}
