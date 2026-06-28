using GeneralLibrary;
using LuceneLibrary;
using Quartz;
using QueueLibrary;

namespace SchedulerWorkerService.Jobs
{

    [DisallowConcurrentExecution]
    public class LuceneIndexCvsJob(ILuceneIndexService luceneIndexService, IDbQueueService queueService, ILogger<LuceneIndexCvsJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("LuceneIndexCvsJob executing at: {time}", DateTimeOffset.Now);

            try
            {
                bool hasMore = true;

                while (hasMore)
                {
                    var job = await queueService.DequeueAsync("index cv", "LuceneIndexCvsJob");

                    if (job == null) { hasMore = false; break; }

                    try
                    {
                        int candidateId = int.Parse(job.payload);
                        await luceneIndexService.IndexCandidate(candidateId);

                        await queueService.CompleteAsync(job.id);
                        Console.WriteLine($"Queue indexed candidate {candidateId}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Queue index failed: {ex.Message}");
                        await queueService.FailAsync(job.id);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                EventViewerWriter.ErrorMessage("LuceneIndexCvsJob was cancelled.");
                logger.LogWarning("LuceneIndexCvsJob was cancelled.");
            }
            catch (Exception ex)
            {
                EventViewerWriter.ErrorMessage("LuceneIndexCvsJob failed." + ex.ToString());
                logger.LogError(ex, "LuceneIndexCvsJob failed.");
                throw new JobExecutionException(ex, refireImmediately: false);
            }
        }
    }
}
