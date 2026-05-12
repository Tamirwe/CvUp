using GeneralLibrary;
using ImportCvsLibrary;
using Quartz;
using System.Xml.Linq;

namespace SchedulerWorkerService.Jobs
{
    [DisallowConcurrentExecution] // Prevents overlapping runs
    public class ImportGmailCvsJob(IImportCvs importCvs, ILogger<ImportGmailCvsJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("ImportGmailCvsJob executing at: {time}", DateTimeOffset.Now);

            try
            {
                await importCvs.ImportFromGmail();
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("ImportGmailCvsJob was cancelled.");
                EventViewerWriter.ErrorMessage("ImportGmailCvsJob was cancelled.");
            }
            catch (Exception ex)
            {
                EventViewerWriter.ErrorMessage("ImportGmailCvsJob failed." +  ex.ToString());

                logger.LogError(ex, "ImportGmailCvsJob failed.");
                // Optionally rethrow to let Quartz handle retries
                throw new JobExecutionException(ex, refireImmediately: false);
            }
        }
    }
}
