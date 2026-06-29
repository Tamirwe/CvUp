namespace OpenAiLibrary
{
    public interface IEmbeddingOpenAi
    {
        Task<float[]?> EmbedText(string? text);
    }
}
