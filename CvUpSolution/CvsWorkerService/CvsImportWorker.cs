using ImportCvsLibrary;

namespace CvsWorkerService
{
    public class CvsImportWorker : BackgroundService
    {
        private readonly IImportCvs _importCvs;
        private readonly ILogger<CvsImportWorker> _logger;

        public CvsImportWorker(IImportCvs importCvs, ILogger<CvsImportWorker> logger)
        {
            _importCvs = importCvs;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                _importCvs.ImportFromGmail();

                if (DateTime.Now.Hour == 3 || DateTime.Now.Hour == 16)
                {
#if !DEBUG
                    _importCvs.BackupDataBase();
#endif
                }

                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}