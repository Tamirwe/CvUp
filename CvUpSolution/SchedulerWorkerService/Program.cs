
using CandsPositionsLibrary;
using AnalyzeEmbedOpenAiLibrary;
using CvFilesLibrary;
using DataModelsLibrary.Queries;
using DotNetEnv.Configuration;
using EmailsLibrary;
using GeneralLibrary;
using GeneralLibrary.IsraelCities;
using Google.Api;
using ImportCvsLibrary;
using LuceneLibrary;
using QueueLibrary;
using Newtonsoft.Json;
using PgVectorLibrary;
using Quartz;
using SchedulerWorkerService.Jobs;
using System.Text;

// Register the code pages provider to support legacy encodings like windows-1255 (Hebrew)
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = Host.CreateApplicationBuilder(args);

// 1. Force the file path to be evaluated relative to the application binaries (not C:\Windows\System32\.env)
string envPath = Path.Combine(AppContext.BaseDirectory, ".env");

// This cleanly appends the file directly to the builder configuration instance
builder.Configuration.AddDotNetEnv(envPath);

// This line is the key one for Windows Service support
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "a Cvs Scheduler Service";
});

builder.Services.AddMemoryCache();

// ✅ always resolves relative to the exe location
var basePath = AppContext.BaseDirectory;
var israeliCitiesString = File.ReadAllText(Path.Combine(basePath, "IsraelCities", "israeliCities.json"));

List<IsraeliCitiesModel> citiesRegionList = JsonConvert.DeserializeObject<List<IsraeliCitiesModel>>(israeliCitiesString)!;

builder.Services.AddSingleton(citiesRegionList);
builder.Services.AddTransient<IAiQueries, AiQueries>();
builder.Services.AddTransient<IAnalyzeCvOpenAi, AnalyzeCvOpenAi>();
builder.Services.AddTransient<ICandsCvsQueries, CandsCvsQueries>();
builder.Services.AddTransient<ILuceneSearchService, LuceneSearchService>();
builder.Services.AddTransient<ILuceneQueries, LuceneQueries>();
builder.Services.AddTransient<ILuceneIndexService, LuceneIndexService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IEmailQueries, EmailQueries>();
builder.Services.AddTransient<ICvsFilesService, CvsFilesService>();
builder.Services.AddTransient<IPositionsQueries, PositionsQueries>();
builder.Services.AddTransient<ICandsListsQueries, CandsListsQueries>();
builder.Services.AddTransient<IAnalyzePositionOpenAi, AnalyzePositionOpenAi>();
builder.Services.AddTransient<ICandsServise, CandsServise>();
builder.Services.AddTransient<ICandsListsServise, CandsListsServise>();
builder.Services.AddTransient<IPositionsServise, PositionsServise>();
builder.Services.AddTransient<IQueueQueries, QueueQueries>();
builder.Services.AddTransient<IDbQueueService, DbQueueService>();
builder.Services.AddTransient<IImportCvs, ImportCvs>();
builder.Services.AddTransient<IDataBaseBackup, DataBaseBackup>();
builder.Services.AddTransient<IAnalyzeCvsService, AnalyzeCvsService>();
builder.Services.AddTransient<IAnalyzePositionsService, AnalyzePositionsService>();
builder.Services.AddTransient<IEmbeddingOpenAi, EmbeddingOpenAi>();
builder.Services.AddTransient<IGenerateAnalyzedCvTextForEmbedding, GenerateAnalyzedCvTextForEmbedding>();
builder.Services.AddTransient<ISearchCvsOpenAi, SearchCvsOpenAi>();
builder.Services.AddTransient<ISearchCvsService, SearchCvsService>();

EventViewerWriter.InfoMessage($"Scheduler started at: {DateTimeOffset.Now}");

bool isDebugMode = builder.Environment.IsDevelopment();

if (isDebugMode)
{
    // ****** Debug: jobs fire immediately on startup
    builder.Services.AddQuartz(q =>
    {
        ////// --- Job 1: Import Gmail Cvs  ---
        //var importGmailCvsJobKey = new JobKey("importGmailCvs");

        //q.AddJob<ImportGmailCvsJob>(opts => opts
        //    .WithIdentity(importGmailCvsJobKey)
        //    .WithDescription("Import Cvs from Gmail"));

        //q.AddTrigger(opts => opts
        //    .ForJob(importGmailCvsJobKey)
        //    .WithIdentity("ImportGmailCvs").StartNow());

        //// --- Job 4: AI Analyze New Cvs    ---
        var aiAnalyzeNewCvs = new JobKey("AnalyzeAndIndexNewCvsJob");

        q.AddJob<AnalyzeAndIndexNewCvsJob>(opts => opts
           .WithIdentity(aiAnalyzeNewCvs)
           .WithDescription("AI Analyze New Cvs"));

        // every 2 minutes, between 8:00 AM and 9:50 PM, every day.
        q.AddTrigger(opts => opts
            .ForJob(aiAnalyzeNewCvs)
            .WithIdentity("AnalyzeAndIndexNewCvsJob").StartNow());

        ////// --- Job 5: Lucene Index Cvs    ---
        //var luceneIndexCvs = new JobKey("LuceneIndexCvsJob");

        //q.AddJob<LuceneIndexCvsJob>(opts => opts
        //   .WithIdentity(luceneIndexCvs)
        //   .WithDescription("Lucene Index Cvs"));

        //// every 2 minutes, between 8:00 AM and 9:50 PM, every day.
        //q.AddTrigger(opts => opts
        //    .ForJob(luceneIndexCvs)
        //    .WithIdentity("LuceneIndexCvsJob").StartNow());

        ////// --- Job 6: Queue Cleanup ---
        //var queueCleanup = new JobKey("QueueCleanupJob");

        //q.AddJob<QueueCleanupJob>(opts => opts
        //   .WithIdentity(queueCleanup)
        //   .WithDescription("Queue Cleanup"));

        //q.AddTrigger(opts => opts
        //    .ForJob(queueCleanup)
        //    .WithIdentity("QueueCleanupJob").StartNow());

        //var dataBaseBackup = new JobKey("CvsDataBaseBackup");

        //q.AddJob<DataBaseBackupJob>(opts => opts
        //   .WithIdentity(dataBaseBackup)
        //   .WithDescription("Cvs DataBase Backup"));

        //q.AddTrigger(opts => opts
        //    .ForJob(dataBaseBackup)
        //    .WithIdentity("dataBase-backup").StartNow());
    });
}
else
{
    // ****** Production Quartz
    builder.Services.AddQuartz(q =>
    {
        // --- Job 1: Import Gmail Cvs  ---
        var importGmailCvsJobKey = new JobKey("importGmailCvs");

        q.AddJob<ImportGmailCvsJob>(opts => opts
            .WithIdentity(importGmailCvsJobKey)
            .WithDescription("Import Cvs from Gmail"));

        // Every minute between 7AM-11PM, Sunday to Friday
        q.AddTrigger(opts => opts
            .ForJob(importGmailCvsJobKey)
            .WithIdentity("ImportGmailCvs-WeekdayTrigger")
            .StartNow()
            .WithCronSchedule("0 * 7-22 ? * SUN-FRI"));

        // Every 2 minutes between 7AM-11PM, Saturday
        q.AddTrigger(opts => opts
            .ForJob(importGmailCvsJobKey)
            .WithIdentity("ImportGmailCvs-SaturdayTrigger")
            .StartNow()
            .WithCronSchedule("0 0/2 7-22 ? * SAT"));

        // --- Job 2: Count Cvs send to position for report  ---
        var countCvsSendToPositionJobKey = new JobKey("countCvsSendToPosition");

        q.AddJob<CountCvsSendToPositionJob>(opts => opts
           .WithIdentity(countCvsSendToPositionJobKey)
           .WithDescription("Count Cvs send to position for report"));

        // Every hour between 9 AM and 5 PM
        q.AddTrigger(opts => opts
            .ForJob(countCvsSendToPositionJobKey)
            .WithIdentity("count-Cvs-Send-To-Position")
            .WithCronSchedule("0 0 9-17 * * ?"));

        // --- Job 3: Cvs DataBase Backup   ---
        var dataBaseBackup = new JobKey("CvsDataBaseBackup");

        q.AddJob<DataBaseBackupJob>(opts => opts
           .WithIdentity(dataBaseBackup)
           .WithDescription("Cvs DataBase Backup"));

        // every hour between 1:00 AM and 4:00 AM
        q.AddTrigger(opts => opts
            .ForJob(dataBaseBackup)
            .WithIdentity("dataBase-backup")
            .WithCronSchedule("0 0 1-4 ? * *"));

        //// --- Job 4: AI Analyze New Cvs    ---
        var aiAnalyzeNewCvs = new JobKey("AnalyzeAndIndexNewCvsJob");

        q.AddJob<AnalyzeAndIndexNewCvsJob>(opts => opts
           .WithIdentity(aiAnalyzeNewCvs)
           .WithDescription("AI Analyze New Cvs"));

        // every 2 minutes, between 8:00 AM and 9:50 PM, every day.
        q.AddTrigger(opts => opts
            .ForJob(aiAnalyzeNewCvs)
            .WithIdentity("Ai-Analyze-New-Cvs-SaturdayTrigger")
            .WithCronSchedule("0 0/2 8-21 ? * *"));

// --- Job 6: Queue Cleanup ---
        var queueCleanup = new JobKey("QueueCleanupJob");

        q.AddJob<QueueCleanupJob>(opts => opts
           .WithIdentity(queueCleanup)
           .WithDescription("Queue Cleanup"));

        // once a day at 4:00 AM
        q.AddTrigger(opts => opts
            .ForJob(queueCleanup)
            .WithIdentity("Queue-Cleanup")
            .WithCronSchedule("0 0 4 * * ?"));
    });
}

EventViewerWriter.InfoMessage($"ImportGmailCvsJob executing at: {DateTimeOffset.Now}");

// Host Quartz as a hosted service
builder.Services.AddQuartzHostedService(options =>
{
    // Forces Quartz to wait until IHostApplicationLifetime.ApplicationStarted fires
    options.AwaitApplicationStarted = true;

    // Optional: Ensures running jobs drain gracefully during app shutdown
    options.WaitForJobsToComplete = true;
});

EventViewerWriter.InfoMessage($"ImportGmailCvsJob executing at: {DateTimeOffset.Now}");

//*** No Worker.cs needed at all. AddQuartzHostedService() registers Quartz itself as an IHostedService, which is enough to keep the process running.
//*** The Worker is only useful if you have your own logic that needs to run on a loop 
//***     (like polling something to decide when to trigger a Quartz job). If Quartz owns the scheduling entirely, skip the Worker.

//***  Register your own background worker (optional alongside Quartz)
//*** builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();







//IHost host = Host.CreateDefaultBuilder(args)
//    .ConfigureServices(services =>
//    {
//        services.AddTransient<ILuceneService, LuceneService>();
//        services.AddTransient<IEmailService, EmailService>();
//        services.AddTransient<IEmailQueries, EmailQueries>();
//        services.AddTransient<ICvsFilesService, CvsFilesService>();
//        services.AddTransient<IPositionsQueries, PositionsQueries>();
//        services.AddTransient<ICandsPositionsServise, CandsPositionsServise>();
//        services.AddTransient<IImportCvs, ImportCvs>();
//        services.AddTransient<IDataBaseBackup, DataBaseBackup>();
//        services.AddMemoryCache();
//        services.AddQuartz(q =>
//        {

//            //// --- Job 1: Import Gmail Cvs  ---
//            var importGmailCvsJobKey = new JobKey("importGmailCvs");

//            q.AddJob<ImportGmailCvsJob>(opts => opts
//                .WithIdentity(importGmailCvsJobKey)
//                .WithDescription("Import Cvs from Gmail"));

//            // Every minute between 7AM-11PM, Sunday to Friday
//            q.AddTrigger(opts => opts
//                .ForJob(importGmailCvsJobKey)
//                .WithIdentity("ImportGmailCvs-WeekdayTrigger")
//                .StartNow() // Executes immediately
//                .WithCronSchedule("0 * 7-22 ? * SUN-FRI"));

//            // Every 2 minutes between 7AM-11PM, Saturday
//            q.AddTrigger(opts => opts
//                .ForJob(importGmailCvsJobKey)
//                .WithIdentity("ImportGmailCvs-SaturdayTrigger")
//                .StartNow() // Executes immediately
//                .WithCronSchedule("0 0/2 7-22 ? * SAT"));

           
//        });
//        services.AddQuartzHostedService(q =>
//        {
//            q.WaitForJobsToComplete = true; // Graceful shutdown
//                                            //q.AwaitApplicationStarted = true; // important for services
//        });
//    })
//    .UseWindowsService()
//    .Build();

//await host.RunAsync();