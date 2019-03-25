using AudioCoreApi.Filters;
using AudioCoreApi.Models;
using AudioCoreSerial;
using AudioCoreSerial.C;
using AudioCoreSerial.I;
using AudioCoreSerial.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO;

namespace AudioControllerCore
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
            services.Configure<AudioOptions>(Configuration.GetSection("Audio"));

            services.AddSingleton<ICommunication, RS232>(kernel =>
            {
                var audioOptions = kernel.GetRequiredService<IOptionsMonitor<AudioOptions>>().CurrentValue;
                return new RS232(audioOptions.COMPort, audioOptions.WriteDelay);
            });
            services.AddScoped<IAmplifier, ControlAE6MC>();

            services.AddMvc(config =>
            {
                config.Filters.Add(typeof(ExceptionFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<AudioContext>();

            var context = new AudioContext();
            context.Database.EnsureCreated();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
