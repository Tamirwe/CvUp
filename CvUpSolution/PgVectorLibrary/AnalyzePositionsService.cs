using AnalyzeEmbedOpenAiLibrary;
using DataModelsLibrary.Queries;
using QueueLibrary;

namespace PgVectorLibrary
{
    public class AnalyzePositionsService(IPositionsQueries positionsQueries, IAnalyzePositionOpenAi analyzePositionOpenAi, IEmbeddingOpenAi embeddingOpenAi, IDbQueueService queueService) : IAnalyzePositionsService
    {
        public async Task AnalyzePosition(int positionId, int companyId=154)
        {
            var position = await positionsQueries.GetPosition(positionId, companyId);
            var positionText = string.Join(" ", new[] { position.name, position.descr, position.requirements }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

            var analyzedPosition = await analyzePositionOpenAi.AiAnalyzePosition(positionText);

            if (analyzedPosition != null)
            {
                var embeddingParts = new List<string>();
                if (!string.IsNullOrWhiteSpace(analyzedPosition.Title))
                    embeddingParts.Add(analyzedPosition.Title);
                if (analyzedPosition.SkillsRequired.Count > 0)
                    embeddingParts.Add($"Skills: {string.Join(", ", analyzedPosition.SkillsRequired)}");
                if (analyzedPosition.SkillsPreferred.Count > 0)
                    embeddingParts.Add($"Preferred: {string.Join(", ", analyzedPosition.SkillsPreferred)}");
                if (analyzedPosition.Industries.Count > 0)
                    embeddingParts.Add($"Industries: {string.Join(", ", analyzedPosition.Industries)}");
                if (analyzedPosition.LuceneKeywords.He.Count > 0)
                    embeddingParts.Add(string.Join(" ", analyzedPosition.LuceneKeywords.He));
                analyzedPosition.EmbeddingText = string.Join("\n", embeddingParts);
            }

            var positionEmbedding = await embeddingOpenAi.EmbedText(analyzedPosition?.EmbeddingText);

            if (analyzedPosition != null)
                await positionsQueries.SaveAnalyzedPosition(positionId, analyzedPosition, positionEmbedding);
        }

        public async Task<bool> AnalyzePositionFromQueue()
        {
            var job = await queueService.DequeueAsync("analyze position", "AnalyzePositionsService");

            if (job == null) return false;

            try
            {
                int positionId = int.Parse(job.payload);
                await AnalyzePosition(positionId);
                await queueService.CompleteAsync(job.id);
                Console.WriteLine($"Queue analyzed position {positionId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Queue analyze position failed: {ex.Message}");
                await queueService.FailAsync(job.id);
                return true;
            }
        }
    }
}
