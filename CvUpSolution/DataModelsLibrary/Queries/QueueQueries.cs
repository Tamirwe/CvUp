using Database.models;
using Microsoft.EntityFrameworkCore;

namespace DataModelsLibrary.Queries
{
    public class QueueQueries : IQueueQueries
    {
        public async Task EnqueueAsync(string queueName, string payload, DateTime? visibleAt = null, int maxAttempts = 3)
        {
            using var db = new cvupdbContext();
            db.job_queues.Add(new job_queue
            {
                queue_name   = queueName,
                payload      = payload,
                status       = "pending",
                attempts     = 0,
                max_attempts = maxAttempts,
                created_at   = DateTime.UtcNow,
                visible_at   = visibleAt ?? DateTime.UtcNow,
            });
            await db.SaveChangesAsync();
        }

        public async Task<job_queue?> DequeueAsync(string queueName, string workerId)
        {
            using var db = new cvupdbContext();

            var ids = await db.Database
                .SqlQuery<long>($"""
                    UPDATE job_queue
                    SET    status    = 'processing',
                           locked_at = NOW(),
                           locked_by = {workerId},
                           attempts  = attempts + 1
                    WHERE  id = (
                        SELECT id FROM job_queue
                        WHERE  queue_name = {queueName}
                          AND  status     = 'pending'
                          AND  visible_at <= NOW()
                        ORDER  BY visible_at
                        LIMIT  1
                        FOR UPDATE SKIP LOCKED
                    )
                    RETURNING id
                    """)
                .ToListAsync();

            if (ids.Count == 0) return null;

            return await db.job_queues.FindAsync(ids[0]);
        }

        public async Task CompleteAsync(long jobId)
        {
            using var db = new cvupdbContext();
            await db.Database.ExecuteSqlRawAsync(
                $"UPDATE job_queue SET status = 'completed', locked_at = NOW() WHERE id = {jobId}");
        }

        public async Task FailAsync(long jobId, DateTime? retryVisibleAt = null)
        {
            using var db = new cvupdbContext();
            var job = await db.job_queues.FindAsync(jobId);
            if (job == null) return;

            if (job.attempts >= job.max_attempts)
            {
                await db.Database.ExecuteSqlRawAsync(
                    $"UPDATE job_queue SET status = 'failed', locked_at = NOW() WHERE id = {jobId}");
            }
            else if (retryVisibleAt.HasValue)
            {
                await db.Database.ExecuteSqlRawAsync(
                    $"UPDATE job_queue SET status = 'pending', locked_at = NULL, locked_by = NULL, visible_at = '{retryVisibleAt.Value:O}' WHERE id = {jobId}");
            }
            else
            {
                await db.Database.ExecuteSqlRawAsync(
                    $"UPDATE job_queue SET status = 'pending', locked_at = NULL, locked_by = NULL, visible_at = NOW() WHERE id = {jobId}");
            }
        }

        public async Task CleanupAsync(string queueName, int olderThanDays = 7)
        {
            using var db = new cvupdbContext();
            await db.Database.ExecuteSqlRawAsync(
                $"DELETE FROM job_queue WHERE queue_name = '{queueName}' AND status IN ('completed', 'failed') AND created_at < NOW() - INTERVAL '{olderThanDays} days'");
        }
    }
}
