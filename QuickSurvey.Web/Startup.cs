using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuickSurvey.Core.SessionAggregate;
using QuickSurvey.Infrastructure;
using QuickSurvey.Infrastructure.Repositories;
using QuickSurvey.Web.Authentication;
using QuickSurvey.Web.SignalRCore;
using QuickSurvey.Web.SignalRCore.Hubs;

namespace QuickSurvey.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

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

            services.AddHttpContextAccessor();

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);


            services.AddSignalR();

            services.AddDbContextPool<SurveyContext>(options =>
            {
                options.UseSqlServer(SurveyContext.ConnectionString);
                if (_env.IsDevelopment())
                {
                    options.LogTo(Console.WriteLine, LogLevel.Information);
                    options.EnableSensitiveDataLogging();
                }
            });
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddSingleton<BasicObfuscator>();
            services.AddSingleton<IUserConnectionRepository, InMemoryUserConnectionRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            if (!_env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

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

            if (_env.IsDevelopment())
            {
                app.UseSpa(spa =>
                {
                    if (_env.IsDevelopment())
                    {
                        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                    }
                });
            }

            app.Map(new PathString("/App"), appMember =>
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
