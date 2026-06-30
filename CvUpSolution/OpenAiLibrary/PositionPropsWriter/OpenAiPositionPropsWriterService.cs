using DataModelsLibrary.Models;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace OpenAiLibrary.PositionPropsWriter
{
    public class OpenAiPositionPropsWriterService : IOpenAiPositionPropsWriterService
    {
        private ChatClient? _chatClient;
        private readonly string? _apiKey;

        private string? _requirementsPrompt;
        private string? _descriptionPrompt;
        private string? _jobAdPrompt;

        public OpenAiPositionPropsWriterService(IConfiguration configuration)
        {
            _apiKey = configuration["API_KEY"];
        }

        private ChatClient ChatClient =>
            _chatClient ??= new OpenAIClient(_apiKey).GetChatClient("gpt-4o-mini");

        private string RequirementsPrompt =>
            _requirementsPrompt ??= File.ReadAllText(
                Path.Combine(AppContext.BaseDirectory, "position_requirements_prompt.txt"));

        private string DescriptionPrompt =>
            _descriptionPrompt ??= File.ReadAllText(
                Path.Combine(AppContext.BaseDirectory, "position_description_prompt.txt"));

        private string JobAdPrompt =>
            _jobAdPrompt ??= File.ReadAllText(
                Path.Combine(AppContext.BaseDirectory, "position_job_ad_prompt.txt"));

        // ── public methods ────────────────────────────────────────────────

        public async Task<PositionContentModel?> OpenAiRewritePositionDescrRequirements(string title, string? requirements, string? description)
        {
            var userContent = BuildUserContent(title, requirements, description);
            var reqTask  = CallAsync(RequirementsPrompt, userContent);
            var descTask = CallAsync(DescriptionPrompt,  userContent);

            await Task.WhenAll(reqTask, descTask);

            return new PositionContentModel
            {
                Requirements = reqTask.Result,
                Description  = descTask.Result,
            };
        }

        public Task<string?> OpenAiPositionAdWriter(string title, string? requirements, string? description) =>
            CallAsync(JobAdPrompt, BuildUserContent(title, requirements, description));

        // ── private helpers ───────────────────────────────────────────────

        private static string BuildUserContent(
            string title,
            string? requirements = null,
            string? description = null)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Job Title: {title}");

            if (!string.IsNullOrWhiteSpace(requirements))
                sb.AppendLine($"\nRequirements:\n{requirements}");

            if (!string.IsNullOrWhiteSpace(description))
                sb.AppendLine($"\nDescription:\n{description}");

            return sb.ToString();
        }

        private async Task<string?> CallAsync(string systemPrompt, string userContent)
        {
            try
            {
                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage(systemPrompt),
                    new UserChatMessage(userContent)
                };

                var options = new ChatCompletionOptions
                {
                    Temperature = 0.4f,
                };

                var completion = await ChatClient.CompleteChatAsync(messages, options);
                return completion.Value.Content[0].Text?.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OpenAiPositionPropsWriterService] Error: {ex.Message}");
                return null;
            }
        }
    }
}
