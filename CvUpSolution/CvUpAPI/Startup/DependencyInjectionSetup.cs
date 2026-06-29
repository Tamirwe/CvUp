using AuthLibrary;
using CandsPositionsLibrary;
using CustomersContactsLibrary;
using CvFilesLibrary;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using FoldersLibrary;
using LuceneLibrary;
using OpenAiLibrary;
using PgVectorLibrary;
using QueueLibrary;

namespace CvUpAPI.Startup
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ILuceneSearchService, LuceneSearchService>();
            services.AddTransient<IEmailQueries, EmailQueries>();
            services.AddTransient<IAuthQueries, AuthQueries>();
            services.AddTransient<IAuthServise, AuthServise>();
            services.AddTransient<IFoldersQueries, FoldersQueries>();
            services.AddTransient<IContactsQueries, ContactsQueries>();
            services.AddTransient<IFoldersService, FoldersService>();
            services.AddTransient<ICvsFilesService, CvsFilesService>();
            services.AddTransient<ICustomersContactsService, CustomersContactsService>();
            services.AddTransient<IQueueQueries, QueueQueries>();
            services.AddTransient<IDbQueueService, DbQueueService>();
            services.AddTransient<IOpenAiAnalyzePosition, OpenAiAnalyzePosition>();
            services.AddTransient<IOpenAiEmbedding, OpenAiEmbedding>();
            services.AddTransient<IAnalyzePositionsService, AnalyzePositionsService>();
            services.AddTransient<ICandsServise, CandsServise>();
            services.AddTransient<ICandsListsServise, CandsListsServise>();
            services.AddTransient<IPositionsServise, PositionsServise>();
            services.AddTransient<IPositionsQueries, PositionsQueries>();
            services.AddTransient<ICandsCvsQueries, CandsCvsQueries>();
            services.AddTransient<ICandsListsQueries, CandsListsQueries>();
            services.AddTransient<ITranslateService, TranslateService>();
            services.AddTransient<IAiQueries, AiQueries>();
            services.AddTransient<IOpenAiSearchCvs, OpenAiSearchCvs>();
            services.AddTransient<ISearchCvsService, SearchCvsService>();

            return services;
        }
    }
}
