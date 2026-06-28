using GeneralLibrary;
using LuceneLibrary;
using PgVectorLibrary;
using Quartz;
using QueueLibrary;

namespace SchedulerWorkerService.Jobs
{

    [DisallowConcurrentExecution] // Prevents overlapping runs
    public class AiAnalyzeNewCvsJob(IAnalyzeCvsService analyzeCvsService, IDbQueueService queueService, ILuceneIndexService luceneIndexService, ILogger<AiAnalyzeNewCvsJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("AiAnalyzeNewCvsJob executing at: {time}", DateTimeOffset.Now);

            try
            {
                bool hasMore = true;

                while (hasMore)
                {
                    var job = await queueService.DequeueAsync("analyze new cv", "AnalyzeCvsService");

                    if (job == null) { hasMore = false; break; }

                    try
                    {
                        int candidateId = int.Parse(job.payload);
                        await analyzeCvsService.AnalyzeCandidates(candidateId);
                        await luceneIndexService.IndexCandidate(candidateId);

                        await queueService.CompleteAsync(job.id);
                        Console.WriteLine($"Queue analyzed candidate {candidateId}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Queue analyze failed: {ex.Message}");
                        await queueService.FailAsync(job.id);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                EventViewerWriter.ErrorMessage("AiAnalyzeNewCvsJob was cancelled.");
                logger.LogWarning("AiAnalyzeNewCvsJob was cancelled.");
            }
            catch (Exception ex)
            {
                EventViewerWriter.ErrorMessage("AiAnalyzeNewCvsJob failed." + ex.ToString());
                logger.LogError(ex, "AiAnalyzeNewCvsJob failed.");
                throw new JobExecutionException(ex, refireImmediately: false);
            }
        }
    }
}
