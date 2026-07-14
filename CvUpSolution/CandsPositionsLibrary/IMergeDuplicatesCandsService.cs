using DataModelsLibrary.Models;

namespace CandsPositionsLibrary
{
    public interface IMergeDuplicatesCandsService
    {
        Task<List<DuplicateEmailCandModel>> MergeDuplicateCandsByEmail(string email = "");
    }
}
