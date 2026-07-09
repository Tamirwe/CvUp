using DataModelsLibrary.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using OpenAI;
using OpenAI.Chat;

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

        public async Task<PositionSearchTermsModel?> GetPositionSearchTerms(string title, string descr, string requirements)
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

            return new PositionSearchTermsModel
            {
                LuceneKeywords = obj["lucene_keywords"]?.ToObject<PositionLuceneKeywordsModel>() ?? new(),
                SearchPhrase = obj.Value<string>("search_phrase"),
            };
        }
    }
}
