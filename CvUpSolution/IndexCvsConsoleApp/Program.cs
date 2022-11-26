// See https://aka.ms/new-console-template for more information
using CvsPositionsLibrary;
using DataModelsLibrary.Queries;
using LuceneLibrary;
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
        services.AddTransient<ILuceneService, LuceneService>();
        services.AddTransient<ICvsPositionsQueries, CvsPositionsQueries>();
        services.AddTransient<ICvsPositionsServise, CvsPositionsServise>();
    })
    .Build();

var cvsPositionsServise = host.Services.GetRequiredService<ICvsPositionsServise>();
cvsPositionsServise.IndexCompanyCvs(132);

