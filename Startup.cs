﻿using App.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace App
{
    public class Startup
    {
        /// <summary>
        /// Application configuration.
        /// </summary>
        /// <returns></returns>
        private IConfiguration _configuration;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="serviceCollection"></param>
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            // ===== Strongly typed application settings =====
            serviceCollection.AddSingleton<IConfiguration>(_configuration);
            serviceCollection.Configure<AppSettings>(_configuration.GetSection("AppSettings"));

            // ===== Add database service =====
            serviceCollection.AddDatabaseService();

            // ===== Add authentication service =====
            serviceCollection.AddAuthenticationService();

            // ===== Add MVC service =====
            serviceCollection.AddMvc()
                .AddJsonOptions(options => {
                    // ===== Use camel case =====
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };
                });

            // ===== Add CORS service =====
            serviceCollection.AddCors(options => {
                options.AddPolicy("default", policy => {
                    // ===== Allow all =====
                    policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                });
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="hostingEnvironment"></param>
        public void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
                applicationBuilder.UseDeveloperExceptionPage();

            applicationBuilder.UseAuthentication();

            applicationBuilder.UseMiddleware<HttpExceptionMiddleware>();

            applicationBuilder.UseMvc();
        }
    }
}
