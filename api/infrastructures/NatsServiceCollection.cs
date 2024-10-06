using infrastructures.Services.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using NATS.Client.Core;
using NATS.Client.Serializers.Json;

namespace infrastructures
{
    public static class NatsServiceCollection
    {
        public static IServiceCollection AddNatsService(this IServiceCollection services, IConfiguration configuration, System.Reflection.Assembly assembly)
        {
            if (bool.Parse(configuration["NATS:Enable"]))
            {
                string natsHost = configuration["NATS:Host"];
                int natsPort = Convert.ToInt32(configuration["NATS:Port"]);
                string natsUsername = configuration["NATS:Username"];
                string natsPassword = configuration["NATS:Password"];
                var options = new NatsOpts { SerializerRegistry = NatsJsonSerializerRegistry.Default, Url = $"nats://{natsUsername}:{natsPassword}@{natsHost}:{natsPort}" };
                services.AddSingleton(typeof(NatsConnection), new NatsConnection());
                services.AddPubsub(assembly);
            }

            return services;
        }

        public static IServiceCollection AddPubsub(this IServiceCollection services, System.Reflection.Assembly assembly)
        {
            assembly.GetTypes()
            .Where(t => !t.IsInterface && t != typeof(IBasePubsub) && t != typeof(BasePubsub) && typeof(IBasePubsub).IsAssignableFrom(t))
            .ToList()
            .ForEach(assignedTypes =>
            {
                var serviceType = assignedTypes.GetInterfaces().First(i => i != typeof(IBasePubsub));
                services.AddScoped(serviceType, assignedTypes);
            });
            return services;
        }
    }
}