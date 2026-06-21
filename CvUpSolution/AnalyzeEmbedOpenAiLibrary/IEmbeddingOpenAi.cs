namespace AnalyzeEmbedOpenAiLibrary
{
    public interface IEmbeddingOpenAi
    {
        Task<float[]?> EmbedText(string? text);
    }
}
