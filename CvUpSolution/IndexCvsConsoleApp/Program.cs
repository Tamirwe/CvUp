// See https://aka.ms/new-console-template for more information
using CandsPositionsLibrary;
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
        services.AddTransient<ICandsPositionsQueries, CandsPositionsQueries>();
        services.AddTransient<ICandsPositionsServise, CandsPositionsServise>();
    })
    .Build();

var cvsPositionsServise = host.Services.GetRequiredService<ICandsPositionsServise>();
cvsPositionsServise.IndexCompanyCvs(132);

