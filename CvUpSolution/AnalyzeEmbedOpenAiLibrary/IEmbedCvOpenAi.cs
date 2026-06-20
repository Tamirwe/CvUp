using DataModelsLibrary.Models;

namespace AnalyzeEmbedOpenAiLibrary
{
    public interface IEmbedCvOpenAi
    {
        Task<CvEmbeddings> EmbedCv(AnalyzedCvsForEmbeedingModel analyzeCv);
        Task<float[]?> EmbedText(string? text);
    }
}