using GeneralLibrary;
using LuceneLibrary;
using PgVectorLibrary;
using Quartz;

namespace SchedulerWorkerService.Jobs
{

    [DisallowConcurrentExecution] // Prevents overlapping runs
    public class LuceneIndexCvsJob(ILuceneIndexService luceneIndexService, ILogger<LuceneIndexCvsJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("LuceneIndexCvsJob executing at: {time}", DateTimeOffset.Now);

            try
            {
                while (await luceneIndexService.IndexNewCvFromQueue()) { }
            }
            catch (OperationCanceledException)
            {
                EventViewerWriter.ErrorMessage("LuceneIndexCvsJob was cancelled.");
                logger.LogWarning("LuceneIndexCvsJob was cancelled.");
            }
            catch (Exception ex)
            {
                EventViewerWriter.ErrorMessage("LuceneIndexCvsJob failed." +  ex.ToString());
                logger.LogError(ex, "LuceneIndexCvsJob failed.");
                // Optionally rethrow to let Quartz handle retries
                throw new JobExecutionException(ex, refireImmediately: false);
            }
        }
    }
}
