using GeneralLibrary;
using PgVectorLibrary;
using Quartz;

namespace SchedulerWorkerService.Jobs
{

    [DisallowConcurrentExecution] // Prevents overlapping runs
    public class AiAnalyzeNewCvsJob(IAnalyzeCvsService analyzeCvsService, IEmbedService embedService, ILogger<AiAnalyzeNewCvsJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("AiAnalyzeNewCvsJob executing at: {time}", DateTimeOffset.Now);

            try
            {
                var hasProcessed = false;
                while (await analyzeCvsService.AnalyzeCvFromQueue()) { hasProcessed = true; }
                if (hasProcessed) await embedService.EmbedAnalyzeCvs();
            }
            catch (OperationCanceledException)
            {
                EventViewerWriter.ErrorMessage("AiAnalyzeNewCvsJob was cancelled.");
                logger.LogWarning("AiAnalyzeNewCvsJob was cancelled.");
            }
            catch (Exception ex)
            {
                EventViewerWriter.ErrorMessage("AiAnalyzeNewCvsJob failed." +  ex.ToString());
                logger.LogError(ex, "AiAnalyzeNewCvsJob failed.");
                // Optionally rethrow to let Quartz handle retries
                throw new JobExecutionException(ex, refireImmediately: false);
            }
        }
    }
}
