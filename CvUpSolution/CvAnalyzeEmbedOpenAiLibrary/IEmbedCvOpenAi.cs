namespace CvAnalyzeEmbedOpenAiLibrary
{
    public interface IEmbedCvOpenAi
    {
        Task<float[]> EmbedCv(string text);
    }
}