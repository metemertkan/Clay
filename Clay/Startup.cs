using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clay.Data;
using Clay.Models.Domain;
using Clay.Repositories.Implementations;
using Clay.Repositories.Interfaces;
using Clay.Services.Implementation;
using Clay.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clay
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
            services.AddIdentity<AppIdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<WebDbContext>()
                .AddDefaultTokenProviders();


            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddTransient<IAttemptRepository, EFAttemptRepository>();
            services.AddTransient<ILockRepository, EFLockRepository>();
            services.AddTransient<IUserLockRepository, EFUserLockRepository>();
            services.AddTransient<ILockService, LockService>();
            services.AddTransient<IAttemptService, AttemptService>();
            services.AddTransient<IUserLockService, UserLockService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<WebDbContext>
                    (opt => opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication();

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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            var seeding = new SeedData(app.ApplicationServices);
            seeding.EnsureSeeded();


        }
    }
}
