using EmailsLibrary;
using LuceneLibrary;
using DataModelsLibrary.Queries;
using AuthLibrary;
using CvsPositionsLibrary;

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
            services.AddTransient<ICvsPositionsServise, CvsPositionsServise>();
            services.AddTransient<ICvsPositionsQueries, CvsPositionsQueries>();
            return services;
        }
    }
}
