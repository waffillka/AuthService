using Bookcrossing.Auth.Data.DbContext;
using Bookcrossing.Auth.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bookcrossing.Auth.Data.Configuration
{
    public static class DependenciesConfiguration
    {
        public static void AddBookcrossingAuthData(this IServiceCollection services, string connectionString)
        {
            services.ConfigureSqlContext(connectionString);
        }

        private static void ConfigureSqlContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AuthDbContext>(opts =>
                opts.UseSqlServer(connectionString, b => b.MigrationsAssembly("Bookcrossing.Auth.Data")));
        }
    }
}
