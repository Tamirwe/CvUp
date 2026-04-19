using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAiLibrary;

internal class Program
{
    private static void Main(string[] args)
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

})
.Build();

        var analyzeCvsService = host.Services.GetRequiredService<IAnalyzeCvsService>();
        List<CandCvTxtModel> candidates = analyzeCvsService.GetCandsLastCvText().Result;

    }
}