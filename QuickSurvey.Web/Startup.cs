using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuickSurvey.Web.Hubs;

namespace QuickSurvey.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddHealthChecks();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}");
                endpoints.MapHealthChecks("/health");
                endpoints.MapHub<MessageHub>("/hub");
            });

            app.MapWhen(IsWebpackServer, webpackDevServer => {
                webpackDevServer.UseSpa(spa => {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                });
            });

            if (env.IsDevelopment())
            {
                app.UseSpa(spa =>
                {
                    if (env.IsDevelopment())
                    {
                        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                    }
                });
            }

            app.Map(new PathString("/app"), appMember =>
            {
                appMember.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";
                });
            });
        }

        private static bool IsWebpackServer(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments("/webpack-dev-server") ||
                   context.Request.Path.StartsWithSegments("/__webpack_dev_server__") ||
                   context.Request.Path.StartsWithSegments("/sockjs-node");
        }
    }
}
