using GeneralLibrary;
using LuceneLibrary;
using PgVectorLibrary;
using PgVectorLibrary.AnalyzeCvs;
using Quartz;
using QueueLibrary;

namespace SchedulerWorkerService.Jobs
{

    [DisallowConcurrentExecution] // Prevents overlapping runs
    public class AnalyzeAndIndexNewCvsJob(IAnalyzeCvsService analyzeCvsService, IDbQueueService queueService, ILuceneIndexService luceneIndexService, ILogger<AnalyzeAndIndexNewCvsJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("AnalyzeAndIndexNewCvsJob executing at: {time}", DateTimeOffset.Now);

            try
            {
                bool hasMore = true;

                while (hasMore)
                {
                    var job = await queueService.DequeueAsync("analyze and index new cv", "AnalyzeAndIndexNewCvsJob");

                    if (job == null) { hasMore = false; break; }

                    try
                    {
                        int candidateId = int.Parse(job.payload);
                        await analyzeCvsService.AnalyzeCandidates(candidateId);
                        await luceneIndexService.IndexCandidate(candidateId);

                        await queueService.CompleteAsync(job.id);
                        Console.WriteLine($"Queue analyzed and indexed candidate {candidateId}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Queue analyze and index failed: {ex.Message}");
                        await queueService.FailAsync(job.id);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                EventViewerWriter.ErrorMessage("AnalyzeAndIndexNewCvsJob was cancelled.");
                logger.LogWarning("AnalyzeAndIndexNewCvsJob was cancelled.");
            }
            catch (Exception ex)
            {
                EventViewerWriter.ErrorMessage("AnalyzeAndIndexNewCvsJob failed." + ex.ToString());
                logger.LogError(ex, "AnalyzeAndIndexNewCvsJob failed.");
                throw new JobExecutionException(ex, refireImmediately: false);
            }
        }
    }
}
