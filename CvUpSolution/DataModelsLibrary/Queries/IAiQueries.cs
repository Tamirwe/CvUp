using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IAiQueries
    {
        Task<List<CandCvTxtModel>> GetCandsLastCvText(int companyId = 154, int candidateId = 0);
        Task<List<AnalyzedCvsForEmbeedingModel>> GetAnalyzedCvsForEmbeeding();
        Task AddCandidateAnalyzeCv(ai_analyze_cv analyzeCv);
        Task DeleteCandidateAnalyzeCv(int candidateId);
        Task UpdateCandIsAnalyzed(int candidateId, bool isAnalyzed);
        Task UpdateCvEmbedding(int candidateId, float[] embedding);
        Task<List<CandidateSearchResultModel>> SearchCvsByEmbedding(float[] queryVector, int limit = 20);
    }
}
