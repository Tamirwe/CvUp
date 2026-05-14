
using CandsPositionsLibrary;
using CvFilesLibrary;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using ImportCvsLibrary;
using LuceneLibrary;
//using OpenAiLibrary.AnalyzeCvsAI;
using Quartz;
using SchedulerWorkerService.Jobs;

var builder = Host.CreateApplicationBuilder(args);

// This line is the key one for Windows Service support
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "a Cvs Scheduler Service";
});

builder.Services.AddMemoryCache();

builder.Services.AddTransient<ILuceneService, LuceneService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IEmailQueries, EmailQueries>();
builder.Services.AddTransient<ICvsFilesService, CvsFilesService>();
builder.Services.AddTransient<ICandsPositionsQueries, CandsPositionsQueries>();
builder.Services.AddTransient<ICandsPositionsServise, CandsPositionsServise>();
builder.Services.AddTransient<IImportCvs, ImportCvs>();
builder.Services.AddTransient<IDataBaseBackup, DataBaseBackup>();
//builder.Services.AddTransient<IAnalyzeCvsService, AnalyzeCvsService>();



// Add Quartz
builder.Services.AddQuartz(q =>
{

    //// --- Job 1: Import Gmail Cvs  ---
    var importGmailCvsJobKey = new JobKey("importGmailCvs");

    q.AddJob<ImportGmailCvsJob>(opts => opts
        .WithIdentity(importGmailCvsJobKey)
        .WithDescription("Import Cvs from Gmail"));

    // Every minute between 7AM-11PM, Sunday to Friday
    q.AddTrigger(opts => opts
        .ForJob(importGmailCvsJobKey)
        .WithIdentity("ImportGmailCvs-WeekdayTrigger")
        .StartNow() // Executes immediately
        .WithCronSchedule("0 * 7-22 ? * SUN-FRI"));

    // Every 2 minutes between 7AM-11PM, Saturday
    q.AddTrigger(opts => opts
        .ForJob(importGmailCvsJobKey)
        .WithIdentity("ImportGmailCvs-SaturdayTrigger")
        .StartNow() // Executes immediately
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


    //// --- Job 3: Cvs DataBase Backup   ---
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
    //var aiAnalyzeNewCvs = new JobKey("AiAnalyzeNewCvsJob");

    //q.AddJob<AiAnalyzeNewCvsJob>(opts => opts
    //   .WithIdentity(aiAnalyzeNewCvs)
    //   .WithDescription("AI Analyze New Cvs"));

    //// Every minute between 7AM-11PM, Sunday to Friday
    //q.AddTrigger(opts => opts
    //    .ForJob(aiAnalyzeNewCvs)
    //    .WithIdentity("Ai-Analyze-New-Cvs-WeekdayTrigger")
    //    .WithCronSchedule("0 * 7-22 ? * SUN-FRI"));

    //// Every 2 minutes between 7AM-11PM, Saturday
    //q.AddTrigger(opts => opts
    //    .ForJob(aiAnalyzeNewCvs)
    //    .WithIdentity("Ai-Analyze-New-Cvs-SaturdayTrigger")
    //    .WithCronSchedule("0 0/2 7-22 ? * SAT"));


});

// Host Quartz as a hosted service
builder.Services.AddQuartzHostedService(q =>
{
    q.WaitForJobsToComplete = true; // Graceful shutdown
});


//*** No Worker.cs needed at all. AddQuartzHostedService() registers Quartz itself as an IHostedService, which is enough to keep the process running.
//*** The Worker is only useful if you have your own logic that needs to run on a loop 
//***     (like polling something to decide when to trigger a Quartz job). If Quartz owns the scheduling entirely, skip the Worker.

//***  Register your own background worker (optional alongside Quartz)
//*** builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
