namespace OpenAiLibrary
{
    public interface ISearchCvsOpenAi
    {
        Task<float[]> EmbedSearchQuery(string query);
    }
}
