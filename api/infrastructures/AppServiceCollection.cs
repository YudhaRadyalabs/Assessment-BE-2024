using infrastructures.Services;
using infrastructures.Services.Base;
using Microsoft.Extensions.DependencyInjection;

namespace infrastructures
{
    public static class AppServiceCollection
    {
        public static IServiceCollection AddAppService(this IServiceCollection services, System.Reflection.Assembly assembly)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            var listService = assembly.GetTypes()
            .Where(t => !t.IsInterface && t != typeof(IBaseService) && t != typeof(BaseService) && typeof(IBaseService).IsAssignableFrom(t))
            .ToList();
            
            foreach (var assignedTypes in listService)
            {
                var serviceType = assignedTypes.GetInterfaces().First(i => i != typeof(IBaseService));
                services.AddScoped(serviceType, assignedTypes);
            }
            
            return services;
        }
    }
}