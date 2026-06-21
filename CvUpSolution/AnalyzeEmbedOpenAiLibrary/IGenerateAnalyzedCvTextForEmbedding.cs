using DataModelsLibrary.Models;

namespace AnalyzeEmbedOpenAiLibrary
{
    public interface IGenerateAnalyzedCvTextForEmbedding
    {
        Task<CvEmbeddings> EmbedCv(AnalyzedCvsForEmbeedingModel analyzeCv);
    }
}