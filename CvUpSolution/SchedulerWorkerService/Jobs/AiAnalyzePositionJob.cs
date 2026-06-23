using GeneralLibrary;
using PgVectorLibrary;
using Quartz;

namespace SchedulerWorkerService.Jobs
{
    [DisallowConcurrentExecution]
    public class AiAnalyzePositionJob(IAnalyzePositionsService analyzePositionsService, ILogger<AiAnalyzePositionJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("AiAnalyzePositionJob executing at: {time}", DateTimeOffset.Now);

            try
            {
                while (await analyzePositionsService.AnalyzePositionFromQueue()) { }
            }
            catch (OperationCanceledException)
            {
                EventViewerWriter.ErrorMessage("AiAnalyzePositionJob was cancelled.");
                logger.LogWarning("AiAnalyzePositionJob was cancelled.");
            }
            catch (Exception ex)
            {
                EventViewerWriter.ErrorMessage("AiAnalyzePositionJob failed." + ex.ToString());
                logger.LogError(ex, "AiAnalyzePositionJob failed.");
                throw new JobExecutionException(ex, refireImmediately: false);
            }
        }
    }
}
