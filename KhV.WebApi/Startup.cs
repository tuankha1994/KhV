using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KhV.WebApi.Models;
using KhV.MongoDb.Context;
using MongoDB.Driver;
using KhV.MongoDb.Repos;
using KhV.Ultis;

namespace KhV.WebApi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            Configuration.ConfigLog("KhV.Api is started...");
            #region Register Mongo
            services.Configure<MongoDbSettings>(Configuration.GetSection("mongodb"));
            var mongo = new MongoDbSettings();
            Configuration.GetSection("mongodb").Bind(mongo);
            var mongoClient = new MongoClient(mongo.ConnectionString);
            services.AddSingleton<IMongoClient>(z => mongoClient);
            services.AddSingleton(z => mongoClient.GetDatabase(mongo.DatabaseName));
            services.AddTransient<IProductRepo, ProductRepo>();
            #endregion
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
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseMvc();
        }
       
    }
}
