using GeneralLibrary;
using Quartz;
using QueueLibrary;

namespace SchedulerWorkerService.Jobs
{

    [DisallowConcurrentExecution] // Prevents overlapping runs
    public class QueueCleanupJob(IDbQueueService queueService, ILogger<QueueCleanupJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("QueueCleanupJob executing at: {time}", DateTimeOffset.Now);

            try
            {
                await queueService.CleanupAsync();
            }
            catch (OperationCanceledException)
            {
                EventViewerWriter.ErrorMessage("QueueCleanupJob was cancelled.");
                logger.LogWarning("QueueCleanupJob was cancelled.");
            }
            catch (Exception ex)
            {
                EventViewerWriter.ErrorMessage("QueueCleanupJob failed." +  ex.ToString());
                logger.LogError(ex, "QueueCleanupJob failed.");
                // Optionally rethrow to let Quartz handle retries
                throw new JobExecutionException(ex, refireImmediately: false);
            }
        }
    }
}
