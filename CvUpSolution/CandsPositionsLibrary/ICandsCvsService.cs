using DataModelsLibrary.Models;

namespace CandsPositionsLibrary
{
    public interface ICandsCvsService
    {
        Task<List<AiCvModel>> GetCandsCvsTextParams(int companyId = 154, int candidateId = 0);
        Task<CvsDistinctWordsModel> GetCandsCvsDistinctWords(int companyId = 154, int candidateId = 0);
    }
}
