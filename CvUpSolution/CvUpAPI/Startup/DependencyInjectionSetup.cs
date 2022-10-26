using EmailsLibrary;
using LuceneLibrary;
using ServicesLibrary.Authentication;
using DataModelsLibrary.Queries;

namespace CvUpAPI.Startup
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            //services.AddScoped<IEmailService, EmailService>();
            services.AddTransient<IEmailsImportService, EmailsImportService>();
            services.AddTransient<IEmailSendService, EmailSendService>();

            services.AddTransient<ILuceneService, LuceneService>();
            services.AddTransient<IEmailQueries, EmailQueries>();
            services.AddTransient<IAuthQueries, AuthQueries>();
            services.AddTransient<IAuthServise, AuthServise>();

            return services;
        }
    }
}
