using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace infrastructures
{
    public static class SwaggerServiceCollection
    {
        public static IServiceCollection AddSwaggerService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = configuration["AppName"], Version = "v1", Description = $" Issuer : {configuration["IssuerUri"]}" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }]
            = new[] { "openid", "profile", "email" }
                });

                //options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                //{
                //    Type = SecuritySchemeType.OAuth2,
                //    Flows = new OpenApiOAuthFlows
                //    {

                //        AuthorizationCode = new OpenApiOAuthFlow
                //        {
                //            AuthorizationUrl = new Uri($"{configuration["IssuerUri"]}/connect/authorize"),
                //            TokenUrl = new Uri($"{configuration["IssuerUri"]}/connect/token"),
                //            Scopes = new Dictionary<string, string>
                //            {
                //                { "openid",""},
                //                { "profile", "" },
                //                { "email", "" }
                //            }
                //        }
                //    }
                //});

                //options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            return services;
        }

        public class AuthorizeCheckOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                                   context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

                if (hasAuthorize)
                {
                    operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                    operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                    operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecurityScheme {Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "oauth2"}}]
                            = new[] { "openid", "profile", "email" }
                    }
                };
                }
            }
        }
    }
}
