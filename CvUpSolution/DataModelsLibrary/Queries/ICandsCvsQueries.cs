using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface ICandsCvsQueries
    {
        Task<List<AiCvModel>> GetDistinctCandsCvs(int companyId = 154, int candidateId = 0);
        Task UpdateIsEmbeddedBatch(List<AnalyzedCvsForEmbeedingModel> cvs);
    }
}
