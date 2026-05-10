using ImportCvsLibrary;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerWorkerService.Jobs
{



    [DisallowConcurrentExecution] // Prevents overlapping runs
    public class DataBaseBackupJob(IDataBaseBackup dataBaseBackup, ILogger<DataBaseBackupJob> logger) : IJob
    {
        public  Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("DataBaseBackupJob executing at: {time}", DateTimeOffset.Now);

            try
            {
                dataBaseBackup.BackupDataBase();

            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("DataBaseBackupJob was cancelled.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DataBaseBackupJob failed.");
                // Optionally rethrow to let Quartz handle retries
                throw new JobExecutionException(ex, refireImmediately: false);
            }

            logger.LogInformation("DataBaseBackupJob executed  at: {time}", DateTimeOffset.Now);

            return Task.CompletedTask;

        }
    }
}
