namespace OpenAiLibrary.PositionPropsWriter
{
    public interface IOpenAiPositionPropsWriterService
    {
        Task<string?> OpenAiRewritePositionProps(string title, string? requirements, string? description, PositionPropsRewriteType rewriteType);
    }
}
