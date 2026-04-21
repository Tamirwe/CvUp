using OpenAI.Embeddings;
using OpenAiLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiLibrary.EmbeddingQdrant
{



    public class Embedder
    {
        private readonly EmbeddingClient _client;

        public Embedder(string apiKey)
        {
            _client = new EmbeddingClient(QdrantConfig.EmbeddingModel, apiKey);
        }

        public async Task<float[]> EmbedAsync(string text)
        {
            var result = await _client.GenerateEmbeddingAsync(text);
            return result.Value.ToFloats().ToArray();
        }

        // Build a clean searchable string from the analyzed CV
        public static string BuildEmbedText(AnalyzedCvModel cv) =>
            string.Join(" ",
                cv.CurrentTitle,
                cv.Seniority.ToString(),
                string.Join(" ", cv.Skills),
                cv.Location,
                cv.Summary,
                cv.YearsExperience > 0 ? $"{cv.YearsExperience} years" : ""
            ).Trim();
    }

}
