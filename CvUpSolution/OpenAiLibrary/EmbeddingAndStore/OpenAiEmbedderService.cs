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
                cv.professionWordsEn != null ? string.Join(" ", cv.professionWordsEn) : "",
                cv.professionWordsHe != null ? string.Join(" ", cv.professionWordsHe) : "",
                cv.professionSkillsEn != null ? string.Join(" ", cv.professionSkillsEn) : "",
                cv.professionSkillsHe != null ? string.Join(" ", cv.professionSkillsHe) : "",
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
