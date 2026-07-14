// See https://aka.ms/new-console-template for more information
using CandsPositionsLibrary;
using CvFilesLibrary;
using DataModelsLibrary.Queries;
using DotNetEnv.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Hello, World!");
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // Appends the .env file keys to the IConfiguration pipeline
        config.AddDotNetEnv(".env");
    })
    .ConfigureServices((_, services) =>
    {
        services.AddTransient<IPositionsQueries, PositionsQueries>();
        services.AddTransient<ICvsFilesService, CvsFilesService>();
        services.AddTransient<ICandsCvsQueries, CandsCvsQueries>();
        services.AddTransient<IFoldersQueries, FoldersQueries>();
        services.AddTransient<IMergeDuplicatesCandsService, MergeDuplicatesCandsService>();
    })
    .Build();


//var cvsFilesServise = host.Services.GetRequiredService<ICvsFilesService>();
//await cvsFilesServise.ImportNewCvsExternalDisk(154,@"E:\\CvsFolders");
//cvsFilesServise.RemoveUnRelatedCvsFiles();

var mergeDuplicatesCandsService = host.Services.GetRequiredService<IMergeDuplicatesCandsService>();
await mergeDuplicatesCandsService.MergeDuplicateCandsByEmail();
