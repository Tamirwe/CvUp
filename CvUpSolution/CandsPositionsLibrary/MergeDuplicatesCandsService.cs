using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;

namespace CandsPositionsLibrary
{
    public class MergeDuplicatesCandsService(ICandsCvsQueries candsCvsQueries) : IMergeDuplicatesCandsService
    {
        public async Task<List<DuplicateEmailCandModel>> GetDuplicateCandsByEmail()
        {
            return await candsCvsQueries.GetDuplicateCandsByEmail();
        }
    }
}
