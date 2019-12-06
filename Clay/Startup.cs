﻿using System.Text;
using Microsoft.Extensions.Logging;
using Clay.Data;
using Clay.Managers.Implementations;
using Clay.Managers.Interfaces;
using Clay.Models.Domain;
using Clay.Repositories.Implementations;
using Clay.Repositories.Interfaces;
using Clay.Services.Implementation;
using Clay.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
            services.AddTransient<IUserLockManager, UserLockManager>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddMvcOptions(options => options.AllowEmptyInputInBodyModelBinding = true);

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<WebDbContext>
                    (opt => opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            var key = Configuration.GetSection("Jwt").GetSection("Key").Value;
            var issuer = Configuration.GetSection("Jwt").GetSection("Issuer").Value;

            services.AddAuthentication(x =>
                    {
                        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    }
                 )
                .AddJwtBearer(x =>
                {
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = issuer,
                        ValidAudience = issuer
                    };
                });

            services.AddMemoryCache();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Clay API", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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

            loggerFactory.AddFile("Logs/clay-{Date}.txt");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clay API V1");
            });

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
