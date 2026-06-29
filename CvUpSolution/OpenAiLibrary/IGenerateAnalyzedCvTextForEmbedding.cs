using DataModelsLibrary.Models;

namespace OpenAiLibrary
{
    public interface IGenerateAnalyzedCvTextForEmbedding
    {
        Task<CvEmbeddings> EmbedCv(AnalyzedCvsForEmbeedingModel analyzeCv);
    }
}
