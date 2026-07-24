using DataModelsLibrary.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using OpenAI;
using OpenAI.Chat;
using System.Text.RegularExpressions;

namespace OpenAiLibrary.PositionPropsWriter
{
    public class OpenAiGetPositionSearchTermsService : IOpenAiGetPositionSearchTermsService
    {
        private ChatClient? _chatClient;
        private readonly string? _apiKey;
        private string? _prompt;

        public OpenAiGetPositionSearchTermsService(IConfiguration configuration)
        {
            _apiKey = configuration["API_KEY"];
        }

        private ChatClient ChatClient =>
            _chatClient ??= new OpenAIClient(_apiKey).GetChatClient("gpt-4o-mini");

        private string Prompt =>
            _prompt ??= File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "PositionPropsWriter//position_search_terms_prompt.txt"));

        public async Task<PositionSearchTermsModel?> GetAnalyzedPositionSearchTerms(string title, string descr, string requirements)
        {
            string? json = null;

            try
            {
                var positionText = string.Join(" ", new[] { title, descr, requirements }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));

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

        private static PositionSearchTermsModel ParseResult(string json)
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
            var luceneKeywords = obj["lucene_keywords"]?.ToObject<PositionLuceneKeywordsModel>() ?? new();

            return new PositionSearchTermsModel
            {
                ShouldHaveInIndexSearch = luceneKeywords.He.Concat(luceneKeywords.En).ToList(),
                MustHaveInIndexSearch = CleanTerms(obj["must_have"]),
                AiSearchPrompt = obj.Value<string>("search_phrase"),
            };
        }

        // Keeps letters and single spaces only, so a must have term can never reach the index
        // search carrying a digit or a sign. Accepts either a json array or one comma separated
        // string, splitting on the comma before the rest is stripped.
        private static List<string> CleanTerms(JToken? token)
        {
            var rawTerms = token is JArray array
                ? array.Select(t => t.ToString())
                : [token?.ToString() ?? ""];

            return rawTerms
                .SelectMany(t => t.Split(','))
                .Select(t => Regex.Replace(t, @"[^\p{L}\s]", " "))
                .Select(t => Regex.Replace(t, @"\s+", " ").Trim())
                .Where(t => t.Length > 0)
                .Take(2)
                .ToList();
        }
    }
}
