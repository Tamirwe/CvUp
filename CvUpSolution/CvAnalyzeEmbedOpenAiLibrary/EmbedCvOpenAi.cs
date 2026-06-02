using CvAnalyzeEmbedOpenAiLibrary.Models;
using DataModelsLibrary.Models;
using Microsoft.Extensions.Configuration;
using OpenAI.Embeddings;
using System.Numerics;

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

        public async Task<float[]> EmbedCv(AnalyzedCvsForEmbeedingModel analyzeCv)
        {
            var text = BuildTextForEmbedding(analyzeCv);
            var result = await _client.GenerateEmbeddingAsync(text);
            float[] vector =  result.Value.ToFloats().ToArray();
            return vector;
        }

        private static string BuildTextForEmbedding(AnalyzedCvsForEmbeedingModel analyzeCv) =>
           string.Join(" ",
               analyzeCv.Location ?? "",
               analyzeCv.Region ?? "",
               analyzeCv.Area ?? "",
               analyzeCv.JobsTitlesEn != null ? string.Join(" ", analyzeCv.JobsTitlesEn) : "",
               analyzeCv.JobsTitlesHe != null ? string.Join(" ", analyzeCv.JobsTitlesHe) : "",
               analyzeCv.ProfessionWordsEn != null ? string.Join(" ", analyzeCv.ProfessionWordsEn) : "",
               analyzeCv.ProfessionWordsHe != null ? string.Join(" ", analyzeCv.ProfessionWordsHe) : "",
               analyzeCv.ProfessionSkillsEn != null ? string.Join(" ", analyzeCv.ProfessionSkillsEn) : "",
               analyzeCv.ProfessionSkillsHe != null ? string.Join(" ", analyzeCv.ProfessionSkillsHe) : "",
               analyzeCv.Seniority ?? "",
               analyzeCv.Education ?? "",
               analyzeCv.Companies != null ? string.Join(" ", analyzeCv.Companies) : "",
               analyzeCv.Skills != null ? string.Join(" ", analyzeCv.Skills) : "",
               analyzeCv.MilitaryService ?? "",
               analyzeCv.SummaryEn ?? "",
               analyzeCv.SummaryHe ?? "",
               analyzeCv.YearsExperience > 0 ? $"{analyzeCv.YearsExperience} years" : ""
           ).Trim();

       

    }
}
