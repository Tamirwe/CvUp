namespace OpenAiLibrary
{
    public interface IOpenAiSearchCvs
    {
        Task<float[]> EmbedSearchQuery(string query);
    }
}
