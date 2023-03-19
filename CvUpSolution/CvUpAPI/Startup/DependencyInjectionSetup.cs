using EmailsLibrary;
using LuceneLibrary;
using DataModelsLibrary.Queries;
using AuthLibrary;
using CandsPositionsLibrary;
using FoldersLibrary;
using CustomersContactsServiceLibrary;

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
            services.AddTransient<ICustomersContactsServiceService, CustomersContactsServiceService>();
            services.AddTransient<ICandsPositionsServise, CandsPositionsServise>();
            services.AddTransient<ICandsPositionsQueries, CandsPositionsQueries>();
            return services;
        }
    }
}
