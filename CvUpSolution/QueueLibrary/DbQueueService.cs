using Database.models;
using DataModelsLibrary.Queries;

namespace QueueLibrary
{
    public class DbQueueService : IDbQueueService
    {
        private readonly IQueueQueries _queueQueries;

        public DbQueueService(IQueueQueries queueQueries)
        {
            _queueQueries = queueQueries;
        }

        public Task EnqueueAsync(string queueName, string payload, DateTime? visibleAt = null, int maxAttempts = 3) =>
            _queueQueries.EnqueueAsync(queueName, payload, visibleAt, maxAttempts);

        public Task<job_queue?> DequeueAsync(string queueName, string workerId) =>
            _queueQueries.DequeueAsync(queueName, workerId);

        public Task CompleteAsync(long jobId) =>
            _queueQueries.CompleteAsync(jobId);

        public Task FailAsync(long jobId, DateTime? retryVisibleAt = null) =>
            _queueQueries.FailAsync(jobId, retryVisibleAt);

        public Task CleanupAsync(int olderThanDays = 7) =>
            _queueQueries.CleanupAsync(olderThanDays);
    }
}
