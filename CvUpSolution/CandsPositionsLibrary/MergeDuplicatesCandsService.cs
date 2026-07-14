using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Microsoft.Extensions.Logging;

namespace CandsPositionsLibrary
{
    public class MergeDuplicatesCandsService(ICandsCvsQueries candsCvsQueries, IFoldersQueries foldersQueries, IPositionsQueries positionsQueries, ILogger<MergeDuplicatesCandsService> logger) : IMergeDuplicatesCandsService
    {
        public async Task<List<DuplicateEmailCandModel>> MergeDuplicateCandsByEmail(string email = "")
        {
            var duplicates = string.IsNullOrEmpty(email)
                ? await candsCvsQueries.GetDuplicateCandsByEmail()
                : new List<DuplicateEmailCandModel> { new() { Email = email } };

            foreach (var dup in duplicates)
            {
                await FindCandPrimaryRecord(dup.Email);
            }

            return duplicates;
        }

        private async Task<int?> FindCandPrimaryRecord(string candEmail)
        {
            // Single shared context/connection for this candEmail, so all writes commit or
            // roll back together. A TransactionScope would open a separate connection per
            // query call, which forces Npgsql to escalate to a distributed transaction it
            // doesn't support.
            using var dbContext = new cvupdbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var cands = await candsCvsQueries.GetCandsByEmail(candEmail, dbContext);

                if (cands.Count <= 1)
                    return null;

                var candIds = cands.Select(c => c.id).ToList();

                var orderedCands = cands.OrderByDescending(c => c.date_updated).ToList();
                var mainCand = orderedCands.First();
                var candMainId = mainCand.id;

                var otherCandIds = candIds.Where(id => id != candMainId).ToList();

                MergeCandReview(orderedCands, mainCand);
                MergeCandDetails(orderedCands, mainCand);

                await candsCvsQueries.UpdateCandidate(mainCand, dbContext);

                await candsCvsQueries.UpdateCvsCandId(candMainId, otherCandIds, dbContext);
                await candsCvsQueries.UpdateCvsTxtCandId(candMainId, otherCandIds, dbContext);
                await candsCvsQueries.UpdateFoldersCandsCandId(candMainId, otherCandIds, dbContext);
                await candsCvsQueries.UpdatePositionCandidatesCandId(candMainId, otherCandIds, dbContext);

                await candsCvsQueries.DeleteCands(otherCandIds, dbContext);

                await transaction.CommitAsync();

                await foldersQueries.UpdateCandidateFolders(companyId: 154, candidateId: candMainId);
                await positionsQueries.UpdateCandPosArrays(companyId: 154, candidateId: candMainId);

                return candMainId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex, "Failed to merge duplicate candidates for email {CandEmail}", candEmail);
                return null;
            }
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
