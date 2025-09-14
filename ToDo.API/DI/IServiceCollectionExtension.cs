using Microsoft.Extensions.Configuration;
using ToDo.API.Configurations;

namespace ToDo.API.DI
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Configurations.ApplicationConfiguration>(configuration.GetSection(nameof(ApplicationConfiguration)));
            return services;
        }
    }
}
