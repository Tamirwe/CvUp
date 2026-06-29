namespace OpenAiLibrary.SearchCvs
{
    public interface IOpenAiSearchCvs
    {
        Task<float[]> EmbedSearchQuery(string query);
    }
}
