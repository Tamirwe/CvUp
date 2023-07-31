using CvsWorkerService;
using ImportCvsLibrary;
using DataModelsLibrary.Queries;
using LuceneLibrary;
using CandsPositionsLibrary;
using CvFilesLibrary;
using EmailsLibrary;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<ILuceneService, LuceneService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IEmailQueries, EmailQueries>();
        services.AddTransient<ICvsFilesService, CvsFilesService>();
        services.AddTransient<ICandsPositionsQueries, CandsPositionsQueries>();
        services.AddTransient<ICandsPositionsServise, CandsPositionsServise>();
        services.AddSingleton<IImportCvs, ImportCvs>();
        services.AddHostedService<CvsImportWorker>();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
