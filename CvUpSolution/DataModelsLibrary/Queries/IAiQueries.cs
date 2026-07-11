using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IAiQueries
    {
        Task<List<CandLastCvModel>> AllCandidatesLastCvNotAnalysed();
        Task<CandLastCvModel?> CandidateLastCvNotAnalysed(int candidateId);
        Task<List<AnalyzedCvsForEmbeedingModel>> GetAnalyzedCvsForEmbeeding(int candidateId = 0);
        Task AddCandidateAnalyzeCv(analyzed_cv analyzeCv);
        Task DeleteCandidateAnalyzeCv(int candidateId);
        Task UpdateCandIsAnalyzed(int candidateId, bool isAnalyzed);
        Task UpdateCvEmbedding(int candidateId, float[]? titlesEmbedding, float[]? skillsEmbedding, float[]? summaryEmbedding, float[]? companiesEmbedding);
        Task<List<AiCandidateSearchModel>> SearchCvsByEmbedding(float[] queryVector, List<int>? candidateIds = null, int limit = 20);
    }
}
