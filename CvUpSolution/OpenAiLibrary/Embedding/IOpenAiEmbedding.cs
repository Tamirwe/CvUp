namespace OpenAiLibrary.Embedding
{
    public interface IOpenAiEmbedding
    {
        Task<float[]?> EmbedText(string? text);
    }
}
