using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface ICandsCvsQueries
    {
        Task<List<AiCvModel>> GetDistinctCandsCvs(int companyId = 154, int candidateId = 0);
        Task<List<CandCvTxtModel>> GetCandsLastCvText(int companyId = 154, int candidateId = 0);
        Task AddCandidateAnalyzeCv(ai_analyze_cv analyzeCv);
        Task DeleteCandidateAnalyzeCv(int candidateId);
        Task UpdateCandIsAnalyzed(int candidateId, bool isAnalyzed);
        Task<List<EmbedCvDataModel>> GetAnalyzedCvsForEmbeeding();
        Task UpdateIsEmbeddedBatch(List<EmbedCvDataModel> cvs);
    }
}
