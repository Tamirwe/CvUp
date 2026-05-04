using DataModelsLibrary.Models;
using OpenAI.Embeddings;
using OpenAiLibrary.Models;

namespace OpenAiLibrary.EmbeddingAndStore
{

    public class OpenAiEmbedderService : IOpenAiEmbedderService
    {
        private readonly EmbeddingClient _client;

        public OpenAiEmbedderService(string apiKey)
        {
            _client = new EmbeddingClient(QdrantConfig.EmbeddingModel, apiKey);
        }

        public async Task<float[]> EmbedAsync(string text)
        {
            var result = await _client.GenerateEmbeddingAsync(text);
            return result.Value.ToFloats().ToArray();
        }

        // Build a clean searchable string from the analyzed CV
        public static string BuildEmbedText(EmbedCvDataModel cv) =>
            string.Join(" ",
                cv.Location ?? "",
                cv.Region ?? "",
                cv.Area ?? "",
                cv.CurrentJobTitleEn,
                cv.CurrentJobTitleHe,
                cv.ProfessionWordsEn != null ? string.Join(" ", cv.ProfessionWordsEn) : "",
                cv.ProfessionWordsHe != null ? string.Join(" ", cv.ProfessionWordsHe) : "",
                cv.ProfessionSkillsEn != null ? string.Join(" ", cv.ProfessionSkillsEn) : "",
                cv.ProfessionSkillsHe != null ? string.Join(" ", cv.ProfessionSkillsHe) : "",
                cv.Seniority ?? "",
                cv.Education ?? "",
                cv.Companies ?? "",
                cv.Skills != null ? string.Join(" ", cv.Skills) : "",
                cv.MilitaryService ?? "",
                cv.SummaryEn ?? "",
                cv.SummaryHe ?? "",
                cv.YearsExperience > 0 ? $"{cv.YearsExperience} years" : ""
            ).Trim();
    }

}
