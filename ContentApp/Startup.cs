using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentApp.Data;
using ContentApp.FileStorage;
using ContentApp.KeyVault;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ContentApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private IWebHostEnvironment CurrentEnv { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            if (CurrentEnv?.IsDevelopment() == true)
            {
                services.AddDbContext<ContentAppDbContext>(options =>
                    options.UseSqlite(Configuration.GetConnectionString("MDConnectionString")));
            }
            else
            {
                services.AddDbContext<ContentAppDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("MDConnectionString")));
            }

            services.AddSingleton<IKeyVaultPiece, KeyVaultPiece>();
            services.AddSingleton<IFileStoragePiece, FileStoragePiece>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            CurrentEnv = env;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
