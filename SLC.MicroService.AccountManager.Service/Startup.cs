using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SLC.MicroService.AccountManager.Service
{
    public class Startup
    {

        //var baseDir = Environment.ContentRootPath;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            var fileInfo = new FileInfo(location.AbsolutePath);
            AppDomain.CurrentDomain.SetData("TemplatesDirectory", Path.Combine($"{fileInfo.Directory.FullName}", "templates"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();  //wwwroot folder

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // Configure the Settings endpoint and require an authorized user.
                endpoints.MapGet("/set", async context =>
                {
                    string fullFilePath = $"{AppDomain.CurrentDomain.GetData("TemplatesDirectory")}\\status.html";
                    var pageHTML = File.ReadAllText(fullFilePath);
                    await context.Response.WriteAsync($"{pageHTML.Replace("$$__title__$$", "Service Status").Replace("$$__info__$$", "SLC.MicroService.AccountManager Service")}");
                });//TODO: Add LDAP Auth

                endpoints.MapGet("/login", async context =>
                {
                    await context.Response.WriteAsync($"Login page");
                });

                // Configure status endpoint, no authorization requirements.
                endpoints.MapGet("/", async context =>
                {
                    var refreshIntervalInSeconds = 1;
                    string fullFilePath = $"{AppDomain.CurrentDomain.GetData("TemplatesDirectory")}\\status.html";
                    var pageHTML = File.ReadAllText(fullFilePath)
                        .Replace("$$__title__$$", "ContactManager Update Service")
                        .Replace("$$__info__$$", "SLC.MicroService.AccountManager Service")
                        .Replace("$$__TIMESTAMP__$$", $"{DateTime.UtcNow}")
                        .Replace("$$__refresh_interval__$$", $"{refreshIntervalInSeconds}");

                    await context.Response.WriteAsync($"{pageHTML}");
                });
            });
        }
    }
}
