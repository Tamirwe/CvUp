using CvAnalyzeEmbedOpenAiLibrary;
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

            foreach (var analyzeCv in analyzedCvsForEmbeedingList)
            {
                if (string.IsNullOrEmpty(analyzeCv.SummaryHe) && string.IsNullOrEmpty(analyzeCv.SummaryEn))
                {
                    continue;
                }

                float[] vector = await _embedCvOpenAi.EmbedCv(analyzeCv);
                await _aiQueries.UpdateCvEmbedding(analyzeCv.CandidateId, vector);
            }
        }
    }
}
