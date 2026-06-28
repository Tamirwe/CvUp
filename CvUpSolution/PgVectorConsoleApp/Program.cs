using AnalyzeEmbedOpenAiLibrary;
using DataModelsLibrary.Queries;
using QueueLibrary;
using DotNetEnv.Configuration;
using GeneralLibrary.IsraelCities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PgVectorLibrary;

var builder = Host.CreateApplicationBuilder(args);

// 1. Force the file path to be evaluated relative to the application binaries (not C:\Windows\System32\.env)
string envPath = Path.Combine(AppContext.BaseDirectory, ".env");

// This cleanly appends the file directly to the builder configuration instance
builder.Configuration.AddDotNetEnv(envPath);

string israeliCitiesString = File.ReadAllText("IsraelCities//israeliCities.json");
List<IsraeliCitiesModel> citiesRegionList = JsonConvert.DeserializeObject<List<IsraeliCitiesModel>>(israeliCitiesString)!;

builder.Services.AddSingleton(citiesRegionList);
builder.Services.AddTransient<IAiQueries, AiQueries>();
builder.Services.AddTransient<IAnalyzeCvOpenAi, AnalyzeCvOpenAi>();
builder.Services.AddTransient<IEmbeddingOpenAi, EmbeddingOpenAi>();
builder.Services.AddTransient<IGenerateAnalyzedCvTextForEmbedding, GenerateAnalyzedCvTextForEmbedding>();
builder.Services.AddTransient<IQueueQueries, QueueQueries>();
builder.Services.AddTransient<IDbQueueService, DbQueueService>();
builder.Services.AddTransient<IAnalyzeCvsService, AnalyzeCvsService>();
builder.Services.AddTransient<IEmbedService, EmbedService>();

var host = builder.Build();

var analyzeCvsService = host.Services.GetRequiredService<IAnalyzeCvsService>();
var embedSaveService = host.Services.GetRequiredService<IEmbedService>();

int candidateId = 0;

await analyzeCvsService.AnalyzeCandidates(candidateId);


