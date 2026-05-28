using GeneralLibrary;
using ImportCvsLibrary;
using Quartz;

namespace SchedulerWorkerService.Jobs
{

    [DisallowConcurrentExecution] // Prevents overlapping runs
    public class DataBaseBackupJob(IDataBaseBackup dataBaseBackup, ILogger<DataBaseBackupJob> logger) : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            EventViewerWriter.InfoMessage($"DataBaseBackupJob executing at: {DateTimeOffset.Now}");
            logger.LogInformation("DataBaseBackupJob executing at: {time}", DateTimeOffset.Now);

            try
            {
                dataBaseBackup.BackupDataBase();

            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("DataBaseBackupJob was cancelled.");
                EventViewerWriter.ErrorMessage("DataBaseBackupJob was cancelled.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DataBaseBackupJob failed.");
                EventViewerWriter.ErrorMessage("DataBaseBackupJob failed." + ex.ToString());

                // Optionally rethrow to let Quartz handle retries
                throw new JobExecutionException(ex, refireImmediately: false);
            }

            logger.LogInformation("DataBaseBackupJob executed  at: {time}", DateTimeOffset.Now);

            return Task.CompletedTask;

        }
    }
}
