using DataModelsLibrary.Models;
using OpenAI.Embeddings;
using OpenAiLibrary.Models;

namespace OpenAiLibrary.EmbeddingAndStore
{

    public class OpenAiEmbedderService: IOpenAiEmbedderService
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
                cv.CurrentTitle,
                cv.Seniority,
                cv.Skills != null? string.Join(" ", cv.Skills ):"",
                cv.Location,
                $"Region {cv.Region}",
                 $"Area {cv.Area}",
                cv.Summary,
                cv.YearsExperience > 0 ? $"{cv.YearsExperience} years" : ""
            ).Trim();
    }

}
