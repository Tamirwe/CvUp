using EmailsLibrary;
using LuceneLibrary;
using ServicesLibrary.UserLogin;
using ServicesLibrary.RegisterCompanyAndUser;
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
            services.AddTransient<IRegistrationQueries, RegistrationQueries>();
            services.AddTransient<IRegisterCompanyAndUserServise, RegisterCompanyAndUserServise>();
            services.AddTransient<ILoginQueries, LoginQueries>();
            services.AddTransient<IUserLoginServise, UserLoginServise>();

            return services;
        }
    }
}
