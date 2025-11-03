using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ASP.NETCore5.Services;
using ASP.NETCore5.Models;
using ASP.NETCore5.Settings;

using Microsoft.Extensions.Caching.SqlServer;



namespace ASP.NETCore5
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
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddTransient<IMailService, MailService>();

            services.AddControllersWithViews();
            services.AddSingleton<IMyService, MyService>();
            //UdemyDB
            services.AddDbContext<UdemyDBContext>(option =>
            option.UseSqlServer(Configuration.GetConnectionString("UdemyDB"))
            );
            //identity database
            services.AddDbContext<AppIdentityDbContext>(option =>
            option.UseSqlServer(Configuration.GetConnectionString("UdemyDB"))
            );

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;

                options.SignIn.RequireConfirmedEmail = true;
            })
               .AddEntityFrameworkStores<AppIdentityDbContext>()  // ✅ DÒNG QUAN TRỌNG NHẤT
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // Khi chưa đăng nhập
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                //options.SlidingExpiration = true;
            });

            string distributed = Configuration["Distributed"];

            switch (distributed)
            {
                case "MEMORY":
                    services.AddDistributedMemoryCache();
                    services.AddSession(options =>
                    {
                        options.IdleTimeout = TimeSpan.FromSeconds(10);
                        options.Cookie.HttpOnly = true;
                        options.Cookie.IsEssential = true;
                    });
                    break;

                case "SQLSERVER":
                    services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = Configuration.GetConnectionString("UdemyDB");
                        options.SchemaName = "dbo";
                        options.TableName = "UdemyCache";
                    });
                    break;
                default:
                    services.AddDistributedMemoryCache();
                    break;
            }
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
            app.Use(async (context, next)=>
                {
                await next();
                if (context.Response.StatusCode ==404)
                    {
                        context.Request.Path = "/Home";
                        await next();
                    }
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
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
