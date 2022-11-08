using EmailsLibrary;
using LuceneLibrary;
using DataModelsLibrary.Queries;
using AuthLibrary;

namespace CvUpAPI.Startup
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            //services.AddScoped<IEmailService, EmailService>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddTransient<ILuceneService, LuceneService>();
            services.AddTransient<IEmailQueries, EmailQueries>();
            services.AddTransient<IAuthQueries, AuthQueries>();
            services.AddTransient<IAuthServise, AuthServise>();

            return services;
        }
    }
}
