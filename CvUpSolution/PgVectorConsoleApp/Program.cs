using DataModelsLibrary.Queries;
using QueueLibrary;
using DotNetEnv.Configuration;
using GeneralLibrary.IsraelCities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using AiLibrary;
using AiLibrary.AnalyzeCvs;
using OpenAiLibrary;
using OpenAiLibrary.Embedding;
using OpenAiLibrary.AnalyzeCv;

var builder = Host.CreateApplicationBuilder(args);

// 1. Force the file path to be evaluated relative to the application binaries (not C:\Windows\System32\.env)
string envPath = Path.Combine(AppContext.BaseDirectory, ".env");

// This cleanly appends the file directly to the builder configuration instance
builder.Configuration.AddDotNetEnv(envPath);

string israeliCitiesString = File.ReadAllText("IsraelCities//israeliCities.json");
List<IsraeliCitiesModel> citiesRegionList = JsonConvert.DeserializeObject<List<IsraeliCitiesModel>>(israeliCitiesString)!;

builder.Services.AddSingleton(citiesRegionList);
builder.Services.AddTransient<IAiQueries, AiQueries>();
builder.Services.AddTransient<IOpenAiAnalyzeCvService, OpenAiAnalyzeCvService>();
builder.Services.AddTransient<IOpenAiEmbeddingService, OpenAiEmbeddingService>();
builder.Services.AddTransient<IQueueQueries, QueueQueries>();
builder.Services.AddTransient<IDbQueueService, DbQueueService>();
builder.Services.AddTransient<IAnalyzeCvsService, AnalyzeCvsService>();

var host = builder.Build();

var analyzeCvsService = host.Services.GetRequiredService<IAnalyzeCvsService>();
int candidateId = 0;

// after method complete you have to call indexing method (await luceneIndexService.IndexAllCandidates();) to index the analyzed CVs for search and retrieval.
await analyzeCvsService.AnalyzeCandidates(candidateId);


