namespace AnalyzeEmbedOpenAiLibrary
{
    public interface ISearchCvsOpenAi
    {
        Task<float[]> EmbedSearchQuery(string query);
    }
}