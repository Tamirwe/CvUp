using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using dotenv.net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAiLibrary.AnalyzeCvsAI;
using OpenAiLibrary.EmbeddingAndStore;
using OpenAiLibrary.Searcher;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
.ConfigureAppConfiguration(app =>
{
    //app.AddJsonFile($"appsettings.json");
})
.ConfigureServices((_, services) =>
{
    DotEnv.Load();
    var envVars = DotEnv.Read();
    var apiKey = envVars["API_KEY"].Trim();
    var host = envVars["QDRANT_HOST"].Trim();
    var port = int.Parse(envVars["QDRANT_PORT"]);

    services.AddTransient<ICandsCvsQueries, CandsCvsQueries>();
    services.AddTransient<IAnalyzeCvsService, AnalyzeCvsService>(sp => new AnalyzeCvsService(sp.GetRequiredService<ICandsCvsQueries>(), apiKey));
    services.AddTransient<IOpenAiEmbedderService, OpenAiEmbedderService>(sp => new OpenAiEmbedderService( apiKey));
    services.AddTransient<IStoreService, StoreService>(sp => new StoreService(sp.GetRequiredService<IOpenAiEmbedderService>(), host, port));
    services.AddTransient<IEmbedderStoreService, EmbedderStoreService>();
    services.AddTransient<ISearcherService, SearcherService>(sp => new SearcherService(sp.GetRequiredService<IOpenAiEmbedderService>(), host, port));

})
.Build();

       

        //var analyzeCvsService = host.Services.GetRequiredService<IAnalyzeCvsService>();
        var embedderStoreService = host.Services.GetRequiredService<IEmbedderStoreService>();
        //var searcherService = host.Services.GetRequiredService<ISearcherService>();

        //await analyzeCvsService.AiAnalyzeAndStoreAllCandidatesLastCvVer2();
        await embedderStoreService.EmbedAnalyzedCvs();
        //await searcherService.DemoSearch();

        Console.WriteLine();
    }

   
}