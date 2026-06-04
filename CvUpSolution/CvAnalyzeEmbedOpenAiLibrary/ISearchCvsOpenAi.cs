namespace CvAnalyzeEmbedOpenAiLibrary
{
    public interface ISearchCvsOpenAi
    {
        Task<float[]> EmbedSearchQuery(string query);
    }
}