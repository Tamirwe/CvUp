namespace OpenAiLibrary
{
    public interface IOpenAiEmbedding
    {
        Task<float[]?> EmbedText(string? text);
    }
}
