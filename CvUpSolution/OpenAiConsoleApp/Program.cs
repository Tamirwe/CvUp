using DataModelsLibrary.Queries;
using dotenv.net;
using DotNetEnv.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAiLibrary.AnalyzeCvsAI;
using OpenAiLibrary.EmbeddingAndStore;
using OpenAiLibrary.Searcher;

internal class Program
{
    private static async Task Main(string[] args)
    {
        DotEnv.Load();

        using IHost host = Host.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((context, config) =>
             {
                 // Appends the .env file keys to the IConfiguration pipeline
                 config.AddDotNetEnv(".env");
             })
            .ConfigureServices((_, services) =>
            {
                services.AddTransient<ICandsCvsQueries, CandsCvsQueries>();
                services.AddTransient<IAnalyzeCvsService, AnalyzeCvsService>();
                services.AddTransient<IOpenAiEmbedderService, OpenAiEmbedderService>();
                services.AddTransient<IStoreService, StoreService>();
                services.AddTransient<IEmbedderStoreService, EmbedderStoreService>();
                services.AddTransient<ISearcherService, SearcherService>();

            })
            .Build();

        var analyzeCvsService = host.Services.GetRequiredService<IAnalyzeCvsService>();
        //var embedderStoreService = host.Services.GetRequiredService<IEmbedderStoreService>();
        //var searcherService = host.Services.GetRequiredService<ISearcherService>();

        await analyzeCvsService.AiAnalyzeNewCvs();
        //await embedderStoreService.EmbedAnalyzedCvs();
        //await searcherService.DemoSearch();

        Console.WriteLine();
    }


}