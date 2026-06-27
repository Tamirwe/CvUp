// See https://aka.ms/new-console-template for more information
using CandsPositionsLibrary;
using CvFilesLibrary;
using DataModelsLibrary.Queries;
using DotNetEnv.Configuration;
using EmailsLibrary;
using LuceneLibrary;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PgVectorLibrary;
using QueueLibrary;
using System.Buffers;

Console.WriteLine("Hello, World!");

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // Appends the .env file keys to the IConfiguration pipeline
        config.AddDotNetEnv(".env");
    })
    .ConfigureServices((_, services) =>
    {
        services.AddTransient<ILuceneQueries, LuceneQueries>();
        services.AddTransient<ILuceneSearchService, LuceneSearchService>();
        services.AddTransient<ILuceneIndexService, LuceneIndexService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IEmailQueries, EmailQueries>();
        services.AddTransient<ICvsFilesService, CvsFilesService>();
        services.AddTransient<IPositionsQueries, PositionsQueries>();
        services.AddTransient<ICandsCvsQueries, CandsCvsQueries>();
        services.AddTransient<IQueueQueries, QueueQueries>();
        services.AddTransient<IDbQueueService, DbQueueService>();
        services.AddTransient<ICandsServise, CandsServise>();
    })
    .Build();

var cvsPositionsServise = host.Services.GetRequiredService<ICandsServise>();
var luceneIndexService = host.Services.GetRequiredService<ILuceneIndexService>();
//await cvsPositionsServise.UpdateCvsAsciiSum(154);

int candidateId = 0;

if (candidateId == 0)
{
    await luceneIndexService.IndexAllCandidates();
}
else
{
    await luceneIndexService.IndexCandidate(candidateId);
}

//// First search (normal)
//var firstResults = await _luceneSearch.Search(companyId, searchVals);

//// User refines — search within those results
//var refinedResults = await _luceneSearch.SearchWithin(
//    firstResults.Select(r => r.Id),
//    newSearchVals
//);


//await cvsPositionsServise.IndexCompanyCvs(154);

//var candsPositionsServise = host.Services.GetRequiredService<ICandsPositionsServise>();
