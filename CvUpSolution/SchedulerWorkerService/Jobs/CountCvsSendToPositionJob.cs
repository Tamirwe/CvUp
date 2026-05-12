using CandsPositionsLibrary;
using Quartz;

namespace SchedulerWorkerService.Jobs
{

    [DisallowConcurrentExecution] // Prevents overlapping runs
    public class CountCvsSendToPositionJob(ICandsPositionsServise candPosService, ILogger<CountCvsSendToPositionJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("CountCvsSendToPositionJob executing at: {time}", DateTimeOffset.Now);

            try
            {
                await candPosService.CalculatePositionTypesCount(154);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("CountCvsSendToPositionJob was cancelled.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CountCvsSendToPositionJob failed.");
                // Optionally rethrow to let Quartz handle retries
                throw new JobExecutionException(ex, refireImmediately: false);
            }
        }
    }
}
