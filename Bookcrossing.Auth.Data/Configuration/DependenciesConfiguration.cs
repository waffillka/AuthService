using Bookcrossing.Auth.Data.DbContext;
using Bookcrossing.Auth.Data.Entities;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bookcrossing.Auth.Data.Configuration
{
    public static class DependenciesConfiguration
    {
        public static void AddBookcrossingAuthData(this IServiceCollection services, string connectionString)
        {
            services.ConfigureSqlContext(connectionString);
            services.AddIdentity(connectionString);
        }

        private static void AddIdentity(this IServiceCollection services, string connectionString)
        {
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequiredLength = 4;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;

            }).AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.UserInteraction = new UserInteractionOptions
                {
                    LogoutUrl = "/Account/Logout",
                    LoginUrl = "/Account/Login",
                    LoginReturnUrlParameter = "returnUrl"
                };
            }).AddAspNetIdentity<User>()
            // this adds the config data from DB (clients, resources, CORS)
            .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = db =>
                        db.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(typeof(DependenciesConfiguration).GetTypeInfo().Assembly.GetName().Name));
                })
            // this adds the operational data from DB (codes, tokens, consents)
            .AddOperationalStore(options =>
             {
                 options.ConfigureDbContext = db =>
                     db.UseSqlServer(connectionString,
                         sql => sql.MigrationsAssembly(typeof(DependenciesConfiguration).GetTypeInfo().Assembly.GetName().Name));

                 // this enables automatic token cleanup. this is optional.
                 options.EnableTokenCleanup = true;
                 // options.TokenCleanupInterval = 15; // interval in seconds. 15 seconds useful for debugging
             });

            builder.AddDeveloperSigningCredential();
        }

        private static void ConfigureSqlContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AuthDbContext>(opts =>
                opts.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(DependenciesConfiguration).GetTypeInfo().Assembly.GetName().Name)));
        }
    }
}
