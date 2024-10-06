using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace infrastructures
{
    public static class AuthServiceCollection
    {
        public static IServiceCollection AddAuthService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = configuration["IssuerUri"];
                //options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("ApiScope", policy =>
            //    {
            //        policy.RequireAuthenticatedUser();
            //    });
            //});

            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy => policy.RequireClaim("policy_user", "admin"));
                options.AddPolicy("enduser", policy => policy.RequireClaim("policy_user", "enduser"));
            });

            return services;
        }
    }
}