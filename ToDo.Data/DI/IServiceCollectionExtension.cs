
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ToDo.Data.Configurations;

namespace ToDo.Data.DI
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddMyApplicationDbContext(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            var migrationsAssembly = typeof(AppDbContext).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(GetDbConfiguration(configurationSection).ConnectionString, optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly(migrationsAssembly);
                optionsBuilder.EnableRetryOnFailure(maxRetryCount: 15);
            }));

            return services;
        }

        private static DbConfiguration GetDbConfiguration(IConfigurationSection configurationSection)
        {
            var server = configurationSection["Server"]!;
            var database = configurationSection["Database"]!;
            return new DbConfiguration(server, database);
        }
    }
}
