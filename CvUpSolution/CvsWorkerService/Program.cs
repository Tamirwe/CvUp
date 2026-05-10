using CandsPositionsLibrary;
using CvFilesLibrary;
using CvsWorkerService;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using Google.Api;
using ImportCvsLibrary;
using LuceneLibrary;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<ILuceneService, LuceneService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IEmailQueries, EmailQueries>();
        services.AddTransient<ICvsFilesService, CvsFilesService>();
        services.AddTransient<ICandsPositionsQueries, CandsPositionsQueries>();
        services.AddTransient<ICandsPositionsServise, CandsPositionsServise>();
        services.AddTransient<IImportCvs, ImportCvs>();
        services.AddTransient<IDataBaseBackup, DataBaseBackup>();
        services.AddMemoryCache();
        //services.AddSingleton<IImportCvs, ImportCvs>();
        services.AddHostedService<CvsImportWorker>();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
