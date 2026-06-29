namespace OpenAiLibrary.Embedding
{
    public interface IOpenAiEmbeddingService
    {
        Task<float[]?> EmbedText(string? text);
    }
}
