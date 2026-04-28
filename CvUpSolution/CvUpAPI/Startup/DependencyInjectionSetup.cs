using AuthLibrary;
using CandsPositionsLibrary;
using CustomersContactsLibrary;
using CvFilesLibrary;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using FoldersLibrary;
using LuceneLibrary;
using Microsoft.Extensions.Hosting;
using ModuleGeneratorLibrary;
using OpenAiLibrary.EmbeddingAndStore;
using OpenAiLibrary.Searcher;

namespace CvUpAPI.Startup
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, WebApplicationBuilder builder,string apiKey, string host, int port)
        {
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ILuceneService, LuceneService>();
            services.AddTransient<IEmailQueries, EmailQueries>();
            services.AddTransient<IAuthQueries, AuthQueries>();
            services.AddTransient<IAuthServise, AuthServise>();
            services.AddTransient<IFoldersQueries, FoldersQueries>();
            services.AddTransient<IContactsQueries, ContactsQueries>();
            services.AddTransient<IFoldersService, FoldersService>();
            services.AddTransient<ICvsFilesService, CvsFilesService>();
            services.AddTransient<ICustomersContactsService, CustomersContactsService>();
            services.AddTransient<ICandsPositionsServise, CandsPositionsServise>();
            services.AddTransient<ICandsPositionsQueries, CandsPositionsQueries>();
            services.AddTransient<ITranslateService, TranslateService>();
            services.AddTransient<IModuleGeneratorService, ModuleGeneratorService>();
            services.AddTransient<IOpenAiEmbedderService, OpenAiEmbedderService>(sp => new OpenAiEmbedderService(apiKey));
            services.AddTransient<IStoreService, StoreService>(sp => new StoreService(sp.GetRequiredService<IOpenAiEmbedderService>(), host, port));
            services.AddTransient<ISearcherService, SearcherService>(sp => new SearcherService(sp.GetRequiredService<IOpenAiEmbedderService>(), host, port));

            return services;
        }
    }
}
