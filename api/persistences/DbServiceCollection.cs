using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace persistences
{
    public static class DbServiceCollection
    {
        public static IServiceCollection AddPostgresqlService<dbContext>(this IServiceCollection services, string connectionString)
            where dbContext : DbContext
        {
            services.AddDbContext<dbContext>(options => options.UseNpgsql(connectionString), ServiceLifetime.Transient);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            return services;
        }
    }
}
