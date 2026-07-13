using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;

namespace CandsPositionsLibrary
{
    public class MergeDuplicatesCandsService(ICandsCvsQueries candsCvsQueries) : IMergeDuplicatesCandsService
    {
        public async Task<List<DuplicateEmailCandModel>> GetDuplicateCandsByEmail()
        {
            var duplicates = await candsCvsQueries.GetDuplicateCandsByEmail();

            foreach (var dup in duplicates)
            {
                await FindCandPrimaryRecord(dup.Email);
            }

            return duplicates;
        }

        private async Task<int> FindCandPrimaryRecord(string candEmail)
        {
            var cands = await candsCvsQueries.GetCandsByEmail(candEmail);

            var candIds = cands.Select(c => c.id).ToList();

            var candMainId = cands.OrderByDescending(c => c.date_updated).First().id;

            var otherCandIds = candIds.Where(id => id != candMainId).ToList();

            await candsCvsQueries.UpdateCvsCandId(candMainId, otherCandIds);

            return candMainId;
        }
    }
}
