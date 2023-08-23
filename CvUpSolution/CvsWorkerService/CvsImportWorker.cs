using CandsPositionsLibrary;
using ImportCvsLibrary;

namespace CvsWorkerService
{
    public class CvsImportWorker : BackgroundService
    {
        private readonly IImportCvs _importCvs;
        private ICandsPositionsServise _candPosService;

        private readonly ILogger<CvsImportWorker> _logger;
        private bool _isRunning = false;
        private bool _isBuStarted = false;
        private int _hour = 0;
        private bool _isHourChanged = false;

        public CvsImportWorker(IImportCvs importCvs, ICandsPositionsServise candPosService, ILogger<CvsImportWorker> logger)
        {
            _importCvs = importCvs;
            _candPosService = candPosService;
            _logger = logger;
            _isRunning = false;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var hour = DateTime.Now.Hour;

                if (hour != _hour)
                {
                    _isHourChanged = true;
                    _hour = hour;
                }

                if (hour > 6 && hour < 23)
                {
                    if (_isHourChanged)
                    {
                        await _candPosService.CalculatePositionTypesCount(154);
                    }

                    if (!_isRunning)
                    {
                        _isRunning = true;

                        await _importCvs.ImportFromGmail();

                        _isRunning = false;
                    }
                }

                if (hour == 1)
                {
                    _isBuStarted = false;
                }

                if (hour == 3 && !_isBuStarted)
                {
                    _isBuStarted = true;
                    _importCvs.BackupDataBase();
                }

                _isHourChanged = false;


                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}