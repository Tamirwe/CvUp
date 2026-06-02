using CvAnalyzeEmbedOpenAiLibrary;
using DataModelsLibrary.Queries;
using DotNetEnv.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PgVectorLibrary;

var builder = Host.CreateApplicationBuilder(args);

// 1. Force the file path to be evaluated relative to the application binaries (not C:\Windows\System32\.env)
string envPath = Path.Combine(AppContext.BaseDirectory, ".env");

// This cleanly appends the file directly to the builder configuration instance
builder.Configuration.AddDotNetEnv(envPath);



builder.Services.AddTransient<ICandsCvsQueries, CandsCvsQueries>();
builder.Services.AddTransient<IAnalyzeCvOpenAi, AnalyzeCvOpenAi>();
builder.Services.AddTransient<IEmbedCvOpenAi, EmbedCvOpenAi>();

var host = builder.Build();

var analyzeCvsService = host.Services.GetRequiredService<IAnalyzeCvsService>();
var embedSaveService = host.Services.GetRequiredService<IEmbedService>();
await analyzeCvsService.AnalyzeCandidatesLastCv();
await embedSaveService.EmbedAnalyzeCvs();

