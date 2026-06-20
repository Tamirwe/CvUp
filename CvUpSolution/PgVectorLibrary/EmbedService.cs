using AnalyzeEmbedOpenAiLibrary;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;

namespace PgVectorLibrary
{
    public class EmbedService : IEmbedService
    {
        private readonly IEmbedCvOpenAi _embedCvOpenAi;
        private readonly IAiQueries _aiQueries;
        private readonly int _companyId;

        public EmbedService(IEmbedCvOpenAi embedCvOpenAi, IAiQueries aiQueries, int companyId = 154)
        {
            _embedCvOpenAi = embedCvOpenAi;
            _aiQueries = aiQueries;
            _companyId = companyId;
        }

        public async Task EmbedAnalyzeCvs()
        {
            List<AnalyzedCvsForEmbeedingModel> analyzedCvsForEmbeedingList = await _aiQueries.GetAnalyzedCvsForEmbeeding();

            int total = analyzedCvsForEmbeedingList.Count, counter = 0;

            foreach (var analyzeCv in analyzedCvsForEmbeedingList)
            {
                CvEmbeddings embeddings = await _embedCvOpenAi.EmbedCv(analyzeCv);
                await _aiQueries.UpdateCvEmbedding(analyzeCv.CandidateId, embeddings.Titles, embeddings.Skills, embeddings.Summary, embeddings.Companies);
                Console.WriteLine($"Embedded candidate {analyzeCv.CandidateId} ({++counter}/{total})");
            }
        }
    }
}
