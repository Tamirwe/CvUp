using Database.models;
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

            var orderedCands = cands.OrderByDescending(c => c.date_updated).ToList();
            var mainCand = orderedCands.First();
            var candMainId = mainCand.id;

            var otherCandIds = candIds.Where(id => id != candMainId).ToList();

            MergeCandReview(orderedCands, mainCand);
            MergeCandDetails(orderedCands, mainCand);

            await candsCvsQueries.UpdateCandidate(mainCand);

            await candsCvsQueries.UpdateCvsCandId(candMainId, otherCandIds);
            await candsCvsQueries.UpdateCvsTxtCandId(candMainId, otherCandIds);

            return candMainId;
        }

        private static void MergeCandReview(List<candidate> orderedCands, candidate mainCand)
        {
            var reviewParts = new List<string>();

            foreach (var cand in orderedCands)
            {
                if (string.IsNullOrWhiteSpace(cand.review)) continue;

                if (reviewParts.Count > 0)
                {
                    reviewParts.Add("");
                    reviewParts.Add("************");
                    reviewParts.Add($"review by duplicate candidate from {cand.date_updated}");
                    reviewParts.Add("");
                }

                reviewParts.Add(cand.review);
            }

            if (reviewParts.Count > 0)
                mainCand.review = string.Join(Environment.NewLine, reviewParts);
        }

        private static void MergeCandDetails(List<candidate> orderedCands, candidate mainCand)
        {
            if (string.IsNullOrWhiteSpace(mainCand.first_name))
                mainCand.first_name = orderedCands.Select(c => c.first_name).FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

            if (string.IsNullOrWhiteSpace(mainCand.last_name))
                mainCand.last_name = orderedCands.Select(c => c.last_name).FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

            if (string.IsNullOrWhiteSpace(mainCand.phone))
                mainCand.phone = orderedCands.Select(c => c.phone).FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

            if (string.IsNullOrWhiteSpace(mainCand.city))
                mainCand.city = orderedCands.Select(c => c.city).FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
        }
    }
}
