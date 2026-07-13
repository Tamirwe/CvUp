using DataModelsLibrary.Models;

namespace CandsPositionsLibrary
{
    public interface IMergeDuplicatesCandsService
    {
        Task<List<DuplicateEmailCandModel>> GetDuplicateCandsByEmail();
    }
}
