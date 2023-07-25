// See https://aka.ms/new-console-template for more information
using CvFilesLibrary;
using DataModelsLibrary.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Hello, World!");
using IHost host = Host.CreateDefaultBuilder(args)
     .ConfigureAppConfiguration(app =>
     {
         app.AddJsonFile($"appsettings.json");
     })
    .ConfigureServices((_, services) =>
    {
        services.AddTransient<ICandsPositionsQueries, CandsPositionsQueries>();
        services.AddTransient<ICvsFilesService, CvsFilesService>();
    })
    .Build();


var cvsFilesServise = host.Services.GetRequiredService<ICvsFilesService>();
await cvsFilesServise.ImportNewCvsExternalDisk(154,@"E:\\CvsFolders");
//cvsFilesServise.RemoveUnRelatedCvsFiles();
