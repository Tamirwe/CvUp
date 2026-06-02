using DataModelsLibrary.Models;

namespace CvAnalyzeEmbedOpenAiLibrary
{
    public interface IEmbedCvOpenAi
    {
        Task<float[]> EmbedCv(AnalyzedCvsForEmbeedingModel analyzeCv);
    }
}