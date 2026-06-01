using CvAnalyzeEmbedOpenAiLibrary.Models;
using Microsoft.Extensions.Configuration;
using OpenAI.Embeddings;

namespace CvAnalyzeEmbedOpenAiLibrary
{
    public class EmbedCvOpenAi: IEmbedCvOpenAi
    {
        private readonly EmbeddingClient _client;
        public const string _embeddingModel = "text-embedding-3-small";

        public EmbedCvOpenAi(IConfiguration configuration)
        {
            var apiKey = configuration["API_KEY"];
            _client = new EmbeddingClient(_embeddingModel, apiKey);
        }

        public async Task<float[]> EmbedCv(string text)
        {
            var result = await _client.GenerateEmbeddingAsync(text);
            return result.Value.ToFloats().ToArray();
        }

        public static string BuildTextForEmbedding(EmbedCvModel cv) =>
           string.Join(" ",
               cv.Location ?? "",
               cv.Region ?? "",
               cv.Area ?? "",
               cv.JobsTitlesEn != null ? string.Join(" ", cv.JobsTitlesEn) : "",
               cv.JobsTitlesHe != null ? string.Join(" ", cv.JobsTitlesHe) : "",
               cv.ProfessionWordsEn != null ? string.Join(" ", cv.ProfessionWordsEn) : "",
               cv.ProfessionWordsHe != null ? string.Join(" ", cv.ProfessionWordsHe) : "",
               cv.ProfessionSkillsEn != null ? string.Join(" ", cv.ProfessionSkillsEn) : "",
               cv.ProfessionSkillsHe != null ? string.Join(" ", cv.ProfessionSkillsHe) : "",
               cv.Seniority ?? "",
               cv.Education ?? "",
               cv.Companies != null ? string.Join(" ", cv.Companies) : "",
               cv.Skills != null ? string.Join(" ", cv.Skills) : "",
               cv.MilitaryService ?? "",
               cv.SummaryEn ?? "",
               cv.SummaryHe ?? "",
               cv.YearsExperience > 0 ? $"{cv.YearsExperience} years" : ""
           ).Trim();
    
    }
}
