using CandsPositionsLibrary;
using ImportCvsLibrary;
using MailKit;
using System.Diagnostics;

namespace CvsWorkerService
{
    public class CvsImportWorker : BackgroundService
    {
        private readonly IImportCvs _importCvs;
        private readonly IDataBaseBackup _dataBaseBackup;
        private ICandsPositionsServise _candPosService;

        private readonly ILogger<CvsImportWorker> _logger;
        private bool _isRunning = false;
        private bool _isBuStarted = false;
        private int _hour = 0;
        private bool _isHourChanged = false;

        public CvsImportWorker(IImportCvs importCvs, IDataBaseBackup dataBaseBackup, ICandsPositionsServise candPosService, ILogger<CvsImportWorker> logger)
        {
            _importCvs = importCvs;
            _dataBaseBackup = dataBaseBackup;
            _candPosService = candPosService;
            _logger = logger;
            _isRunning = false;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
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
                        _dataBaseBackup.BackupDataBase();
                    }

                    _isHourChanged = false;
                }
                catch (Exception ex)
                {
                    using (EventLog eventLog = new())
                    {
                        if (!EventLog.SourceExists("CvUpImport"))
                        {
                            EventLog.CreateEventSource("CvUpImport", "CvUpImport");
                        }

                        eventLog.Source = "CvUpImport";
                        eventLog.WriteEntry(ex.Message + ", " + ex.ToString(), EventLogEntryType.Information);
                    }
                }

                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}