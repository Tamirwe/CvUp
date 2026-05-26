
using CandsPositionsLibrary;
using CvFilesLibrary;
using DataModelsLibrary.Queries;
//using dotenv.net;
using EmailsLibrary;
using GeneralLibrary;
using ImportCvsLibrary;
using LuceneLibrary;
using OpenAiLibrary.AnalyzeCvsAI;
using Quartz;
using SchedulerWorkerService.Jobs;
using DotNetEnv.Configuration;

//DotEnv.Load(); // loads .env from current directory

internal class Program
{
    private static void Main(string[] args)
    {
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
        builder.Services.AddTransient<ICandsCvsQueries, CandsCvsQueries>();
        builder.Services.AddTransient<ILuceneService, LuceneService>();
        builder.Services.AddTransient<IEmailService, EmailService>();
        builder.Services.AddTransient<IEmailQueries, EmailQueries>();
        builder.Services.AddTransient<ICvsFilesService, CvsFilesService>();
        builder.Services.AddTransient<ICandsPositionsQueries, CandsPositionsQueries>();
        builder.Services.AddTransient<ICandsPositionsServise, CandsPositionsServise>();
        builder.Services.AddTransient<IImportCvs, ImportCvs>();
        builder.Services.AddTransient<IDataBaseBackup, DataBaseBackup>();
        builder.Services.AddTransient<IAnalyzeCvsService, AnalyzeCvsService>();

        EventViewerWriter.InfoMessage($"Scheduler started at: {DateTimeOffset.Now}");



        #region DEBUG - Quartz

        //builder.Services.AddQuartz(q =>
        //{
        //    #region Import Gmail Cv's

        //    //var importGmailCvsJobKey = new JobKey("importGmailCvs");

        //    //q.AddJob<ImportGmailCvsJob>(opts => opts
        //    //    .WithIdentity(importGmailCvsJobKey)
        //    //    .WithDescription("Import Cvs from Gmail"));

        //    //q.AddTrigger(opts => opts
        //    //    .ForJob(importGmailCvsJobKey)
        //    //    .WithIdentity("ImportGmailCvs-WeekdayTrigger").StartNow());  // Executes immediately

        //    #endregion

        //    #region Database Backup

        //    var dataBaseBackup = new JobKey("CvsDataBaseBackup");

        //    q.AddJob<DataBaseBackupJob>(opts => opts
        //           .WithIdentity(dataBaseBackup)
        //           .WithDescription("Cvs DataBase Backup"));

        //    q.AddTrigger(opts => opts
        //            .ForJob(dataBaseBackup)
        //            .WithIdentity("dataBase-backup").StartNow()); // Executes immediately

        //    #endregion

        //});

        #endregion

        #region Production - Add Quartz

        builder.Services.AddQuartz(q =>
        {
            #region Import Gmail Cv's Job

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
            //.WithSimpleSchedule(x => x.WithIntervalInSeconds(20).RepeatForever()));

            // Every 2 minutes between 7AM-11PM, Saturday
            q.AddTrigger(opts => opts
                .ForJob(importGmailCvsJobKey)
                .WithIdentity("ImportGmailCvs-SaturdayTrigger")
                .StartNow() // Executes immediately
                .WithCronSchedule("0 0/2 7-22 ? * SAT"));

            #endregion

            #region Count Cvs send to position for report Job

            var countCvsSendToPositionJobKey = new JobKey("countCvsSendToPosition");

            q.AddJob<CountCvsSendToPositionJob>(opts => opts
               .WithIdentity(countCvsSendToPositionJobKey)
               .WithDescription("Count Cvs send to position for report"));

            // Every hour between 9 AM and 5 PM
            q.AddTrigger(opts => opts
                .ForJob(countCvsSendToPositionJobKey)
                .WithIdentity("count-Cvs-Send-To-Position")
                .WithCronSchedule("0 0 9-17 * * ?"));

            #endregion


            #region DataBase Backup Job

            var dataBaseBackup = new JobKey("CvsDataBaseBackup");

            q.AddJob<DataBaseBackupJob>(opts => opts
               .WithIdentity(dataBaseBackup)
               .WithDescription("Cvs DataBase Backup"));

            // every hour between 1:00 AM and 4:00 AM
            // if the db backup file exist it not executed again.
            q.AddTrigger(opts => opts
                .ForJob(dataBaseBackup)
                .WithIdentity("dataBase-backup")
                .WithCronSchedule("0 0 1-4 ? * *"));

            #endregion

            #region  AI Analyze New Cvs Job

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

            #endregion

        });

        #endregion


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
    }
}

