using Kentico.Web.Mvc;
using Kentico.Xperience.Gql.Web.Mvc;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kentico.Xperience
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddKentico();

            services.AddLocalization()
                .AddMvc();

            services.AddGql();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();

                endpoints.Kentico().MapRoutes();
            });
        }
    }
}