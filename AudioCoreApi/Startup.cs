using AudioCoreApi.Filters;
using AudioCoreApi.Models;
using AudioCoreApi.Services;
using AudioCoreSerial;
using AudioCoreSerial.C;
using AudioCoreSerial.I;
using AudioCoreSerial.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace AudioControllerCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

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
            services.AddScoped<ResetService>();

            services.AddMvc(config =>
            {
                config.Filters.Add(typeof(ExceptionFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<AudioContext>(
                options => {
                    var builder = new SqliteConnectionStringBuilder(Configuration.GetConnectionString("DefaultConnection"));
                    builder.DataSource = Path.Combine(HostingEnvironment.ContentRootPath, builder.DataSource);
                    options.UseSqlite(builder.ToString());

                    new AudioContext(options.Options).Database.EnsureCreated();
                },
                ServiceLifetime.Scoped);

            services.AddHostedService<AmplifierCheckerService>();
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
