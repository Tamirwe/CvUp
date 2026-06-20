using DataModelsLibrary.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI;
using OpenAI.Chat;

namespace AnalyzeEmbedOpenAiLibrary
{
    public class AnalyzePositionOpenAi : IAnalyzePositionOpenAi
    {
        private ChatClient? _chatClient;
        private readonly string? _apiKey;
        private string? _prompt;

        public AnalyzePositionOpenAi(IConfiguration configuration)
        {
            _apiKey = configuration["API_KEY"];
        }

        private ChatClient ChatClient =>
            _chatClient ??= new OpenAIClient(_apiKey).GetChatClient("gpt-4o-mini");

        private string Prompt =>
            _prompt ??= File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "position_prompt.txt"));

        public async Task<AnalyzedPositionModel?> AiAnalyzePosition(string positionText)
        {
            string? json = null;

            try
            {
                if (string.IsNullOrWhiteSpace(positionText))
                    return null;

                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage(Prompt),
                    new UserChatMessage(positionText)
                };

                var chatOptions = new ChatCompletionOptions
                {
                    Temperature = 0.2f,
                    ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
                };

                var completion = await ChatClient.CompleteChatAsync(messages, chatOptions);

                json = completion.Value.Content[0].Text;

                return ParseResult(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  problem: {ex.Message}");
                if (json != null)
                    Console.WriteLine($"  json: {json[..Math.Min(200, json.Length)]}");
            }

            return null;
        }

        private static AnalyzedPositionModel ParseResult(string json)
        {
            json = json
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            int start = json.IndexOf('{');
            int end = json.LastIndexOf('}');

            if (start >= 0 && end > start)
                json = json[start..(end + 1)];

            var obj = JObject.Parse(json);

            return new AnalyzedPositionModel
            {
                Title = obj.Value<string>("title"),
                Seniority = obj.Value<string>("seniority"),
                MinYearsExperience = obj["min_years_experience"]?.ToObject<int?>(),
                DegreeRequired = obj.Value<string>("degree_required"),
                HardRequirements = obj["hard_requirements"]?.ToObject<List<string>>() ?? [],
                SkillsRequired = obj["skills_required"]?.ToObject<List<string>>() ?? [],
                SkillsPreferred = obj["skills_preferred"]?.ToObject<List<string>>() ?? [],
                Industries = obj["industries"]?.ToObject<List<string>>() ?? [],
                Languages = obj["languages"]?.ToObject<List<PositionLanguageModel>>() ?? [],
                LuceneKeywords = obj["lucene_keywords"]?.ToObject<PositionLuceneKeywordsModel>() ?? new(),
                EmbeddingText = obj.Value<string>("embedding_text"),
            };
        }
    }
}
