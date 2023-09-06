using EmailsLibrary;
using LuceneLibrary;
using DataModelsLibrary.Queries;
using AuthLibrary;
using CandsPositionsLibrary;
using FoldersLibrary;
using CustomersContactsLibrary;
using CvFilesLibrary;

namespace CvUpAPI.Startup
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, WebApplicationBuilder builder)
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
            return services;
        }
    }
}
