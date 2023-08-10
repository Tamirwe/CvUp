using ImportCvsLibrary;

namespace CvsWorkerService
{
    public class CvsImportWorker : BackgroundService
    {
        private readonly IImportCvs _importCvs;
        private readonly ILogger<CvsImportWorker> _logger;
        private bool _isRunning  = false;
        public CvsImportWorker(IImportCvs importCvs, ILogger<CvsImportWorker> logger)
        {
            _importCvs = importCvs;
            _logger = logger;
            _isRunning = false;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                if (!_isRunning)
                {
                    _isRunning = true;

                    await _importCvs.ImportFromGmail();

                    _isRunning = false;
                }


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