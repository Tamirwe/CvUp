using CvsWorkerService;
using ImportCvsLibrary;
using CvsPositionsLibrary;
using DataModelsLibrary.Queries;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<ICvsPositionsQueries, CvsPositionsQueries>();
        services.AddTransient<ICvsPositionsServise, CvsPositionsServise>();
        services.AddSingleton<IImportCvs, ImportCvs>();
        services.AddHostedService<CvsImportWorker>();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
