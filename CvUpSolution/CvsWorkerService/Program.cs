using CandsPositionsLibrary;
using CvFilesLibrary;
using CvsWorkerService;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using Google.Api;
using ImportCvsLibrary;
using LuceneLibrary;
using QueueLibrary;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<ILuceneSearchService, LuceneSearchService>();
        services.AddTransient<ILuceneQueries, LuceneQueries>();
        services.AddTransient<ILuceneIndexService, LuceneIndexService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IEmailQueries, EmailQueries>();
        services.AddTransient<ICvsFilesService, CvsFilesService>();
        services.AddTransient<ICandsPositionsQueries, CandsPositionsQueries>();
        services.AddTransient<ICandsListsQueries, CandsListsQueries>();
        services.AddTransient<ICandsServise, CandsServise>();
        services.AddTransient<IPositionsServise, PositionsServise>();
        services.AddTransient<IQueueQueries, QueueQueries>();
        services.AddTransient<IDbQueueService, DbQueueService>();
        services.AddTransient<IImportCvs, ImportCvs>();
        services.AddTransient<IDataBaseBackup, DataBaseBackup>();
        services.AddMemoryCache();
        //services.AddSingleton<IImportCvs, ImportCvs>();
        services.AddHostedService<CvsImportWorker>();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
