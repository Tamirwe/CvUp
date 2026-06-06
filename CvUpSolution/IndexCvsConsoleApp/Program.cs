// See https://aka.ms/new-console-template for more information
using CandsPositionsLibrary;
using CvFilesLibrary;
using DataModelsLibrary.Queries;
using DotNetEnv.Configuration;
using EmailsLibrary;
using LuceneLibrary;
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
        services.AddTransient<ILuceneSearchService, LuceneSearchService>();
        services.AddTransient<ILuceneIndexService, LuceneIndexService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IEmailQueries, EmailQueries>();
        services.AddTransient<ICvsFilesService, CvsFilesService>();
        services.AddTransient<ICandsPositionsQueries, CandsPositionsQueries>();
        services.AddTransient<ICandsPositionsServise, CandsPositionsServise>();
    })
    .Build();

var cvsPositionsServise = host.Services.GetRequiredService<ICandsPositionsServise>();
//await cvsPositionsServise.UpdateCvsAsciiSum(154);

await cvsPositionsServise.IndexAllCandidates(154);

//await cvsPositionsServise.IndexCompanyCvs(154);

//var candsPositionsServise = host.Services.GetRequiredService<ICandsPositionsServise>();
