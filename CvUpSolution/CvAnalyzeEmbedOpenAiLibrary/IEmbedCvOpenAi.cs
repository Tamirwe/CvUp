using DataModelsLibrary.Models;

namespace CvAnalyzeEmbedOpenAiLibrary
{
    public interface IEmbedCvOpenAi
    {
        Task<CvEmbeddings> EmbedCv(AnalyzedCvsForEmbeedingModel analyzeCv);
        Task<float[]?> EmbedText(string? text);
    }
}