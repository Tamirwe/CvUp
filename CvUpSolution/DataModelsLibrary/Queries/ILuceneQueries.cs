using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface ILuceneQueries
    {
        Task<List<CandLastCvModel>> AllCandidatesLastCv();
        Task<CandLastCvModel?> CandidateLastCv(int candidateId);
    }
}
