using CandsPositionsLibrary;
using GeneralLibrary;
using Quartz;

namespace SchedulerWorkerService.Jobs
{

    [DisallowConcurrentExecution] // Prevents overlapping runs
    public class SearchTermsCleanupJob(ICandsListsServise candsListsService, ILogger<SearchTermsCleanupJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("SearchTermsCleanupJob executing at: {time}", DateTimeOffset.Now);

            try
            {
                await candsListsService.CleanupOldSearchTerms();
            }
            catch (OperationCanceledException)
            {
                EventViewerWriter.ErrorMessage("SearchTermsCleanupJob was cancelled.");
                logger.LogWarning("SearchTermsCleanupJob was cancelled.");
            }
            catch (Exception ex)
            {
                EventViewerWriter.ErrorMessage("SearchTermsCleanupJob failed." + ex.ToString());
                logger.LogError(ex, "SearchTermsCleanupJob failed.");
                // Optionally rethrow to let Quartz handle retries
                throw new JobExecutionException(ex, refireImmediately: false);
            }
        }
    }
}
