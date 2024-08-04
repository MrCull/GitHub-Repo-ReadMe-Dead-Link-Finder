using GitHubRepoFinder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit;
using System.Net.Http;
using TelemetryLib;
using WebsiteLinksChecker;

namespace DeadLinkFinderWeb
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

            TelemetryConfig telemetryConfig = Configuration.GetSection("TelemetryConfig").Get<TelemetryConfig>();
            services.AddSingleton(telemetryConfig);

            services.AddSingleton<IEmailConfiguration>(Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
            services.AddSingleton(Configuration.GetSection("EmailAddress").Get<TelemetryLib.EmailAddress>());
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ITelemetry, Telemetry>();


            services.AddTransient<HttpClient, HttpClient>();

            services.AddHttpClient();
            services.AddScoped<ILinkGetter, LinkGetter>();
            services.AddScoped<ILinkChecker, LinkChecker>();

            GitHubClient gitHubClient = new(new ProductHeaderValue("GitHub-repo-finder-for-dead-links-in-readmes-web"));
            services.AddTransient(s => gitHubClient);
            services.AddTransient<SearchRepositoriesRequest, SearchRepositoriesRequest>();

            services.AddTransient(s => new GitHubActiveReposFinder(gitHubClient));


            services.AddControllersWithViews();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
