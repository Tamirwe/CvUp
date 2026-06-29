namespace OpenAiLibrary.SearchCvs
{
    public interface IOpenAiSearchCvsService
    {
        Task<float[]> EmbedSearchQuery(string query);
    }
}
