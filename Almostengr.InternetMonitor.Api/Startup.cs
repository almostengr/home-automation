using Almostengr.InternetMonitor.Api.Clients;
using Almostengr.InternetMonitor.Api.Services;
using Almostengr.InternetMonitor.Api.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Almostengr.InternetMonitor.Api
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Almostengr.InternetMonitor.Api", Version = "v1" });
            });

            AppSettings appSettings = Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            services.AddSingleton(appSettings);

            // clients

            services.AddSingleton<IHomeAssistantClient, HomeAssistantClient>();

            // services

            services.AddSingleton<IDdWrtRouterService, DdWrtRouterService>();
            services.AddSingleton<IHdHomeRunService, HdHomeRunService>();
            services.AddSingleton<IHomeAssistantService, HomeAssistantService>();
            services.AddSingleton<ITextFileService, TextFileService>();
            services.AddSingleton<ITranscriptService, SrtTranscriptService>();

            // workers

            services.AddHostedService<TranscriptWorker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Almostengr.InternetMonitor.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
