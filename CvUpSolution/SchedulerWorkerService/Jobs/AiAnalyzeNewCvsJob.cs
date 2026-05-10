using OpenAiLibrary.AnalyzeCvsAI;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerWorkerService.Jobs
{

    [DisallowConcurrentExecution] // Prevents overlapping runs
    public class AiAnalyzeNewCvsJob(IAnalyzeCvsService analyzeCvsService, ILogger<AiAnalyzeNewCvsJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("AiAnalyzeNewCvsJob executing at: {time}", DateTimeOffset.Now);

            try
            {
                await analyzeCvsService.AiAnalyzeNewCvs();
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("AiAnalyzeNewCvsJob was cancelled.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AiAnalyzeNewCvsJob failed.");
                // Optionally rethrow to let Quartz handle retries
                throw new JobExecutionException(ex, refireImmediately: false);
            }
        }
    }
}
