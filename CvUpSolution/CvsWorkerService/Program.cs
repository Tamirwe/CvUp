using CvsWorkerService;
using ImportCvsLibrary;
using CvsPositionsLibrary;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<ICvsPositionsServise, CvsPositionsServise>();
        //services.AddScoped<ICvsPositionsQueries, CvsPositionsQueries>();
        services.AddSingleton<IImportCvs, ImportCvs>();
        services.AddHostedService<CvsImportWorker>();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
