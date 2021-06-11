using System;
using System.Net.Http;

using Kentico.Xperience.Gql.Core;
using Kentico.Xperience.Gql.Schema;
using Kentico.Xperience.Gql.Schema.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

//using Kentico.Xperience.Gql.Schema.Types.Directives;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kentico.Xperience.Gql.Web.Mvc
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds required services to add GraphQL.
        /// </summary>
        public static IServiceCollection AddGql(
            this IServiceCollection services,
            Action<XperienceGqlOptions>? configureOptions = null
            )
        {
            services.AddSingleton<Query>();

            services
                .AddRouting()
                .AddGraphQLServer()
                .AddQueryType<Query>()
                //.AddDirectiveType<NotEmpty>()
                ;

            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            services.Configure<XperienceGqlOptions>(
                configuration.GetSection(XperienceGqlOptions.XperienceGql)
                );

            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }

            services.AddHttpClient<SchemaRefreshService>()
                .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
                {
                    if (serviceProvider.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
                    {
                        return new HttpClientHandler()
                        {
                            ServerCertificateCustomValidationCallback = (message, certificate, chain, errors) => true
                        };
                    }
                    else
                    {
                        return new HttpClientHandler();
                    }
                });

            WebFarmServerTaskInfoProviderWithRefresh.SchemaRefreshService =
                services.BuildServiceProvider()
                    .GetRequiredService<SchemaRefreshService>();

            return services;
        }
    }
}