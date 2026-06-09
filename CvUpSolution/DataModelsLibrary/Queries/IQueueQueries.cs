using Database.models;

namespace DataModelsLibrary.Queries
{
    public interface IQueueQueries
    {
        Task EnqueueAsync(string queueName, string payload, DateTime? visibleAt = null, int maxAttempts = 3);
        Task<job_queue?> DequeueAsync(string queueName, string workerId);
        Task CompleteAsync(long jobId);
        Task FailAsync(long jobId, DateTime? retryVisibleAt = null);
        Task CleanupAsync(string queueName, int olderThanDays = 7);
    }
}
