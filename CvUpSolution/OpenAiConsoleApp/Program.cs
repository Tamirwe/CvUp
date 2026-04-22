using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using dotenv.net;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAiLibrary.AnalyzeCvsAI;
using OpenAiLibrary.EmbeddingQdrant;
using System.Threading.Tasks;

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

    services.AddTransient<ICandsCvsQueries, CandsCvsQueries>();
    services.AddTransient<IAnalyzeCvsService, AnalyzeCvsService>();
    services.AddTransient<IEmbedderStoreService, EmbedderStoreService>();

})
.Build();

        DotEnv.Load();
        var envVars = DotEnv.Read();
        var apiKey = envVars["API_KEY"];

        var analyzeCvsService = host.Services.GetRequiredService<IAnalyzeCvsService>();
        var embedderStoreService = host.Services.GetRequiredService<IEmbedderStoreService>();

        //await analyzeCvsService.AiAnalyzeAndStoreAllCandidatesLastCv(apiKey);
        await embedderStoreService.EmbedAnalyzedCvs(apiKey);

        Console.WriteLine();
    }
}