using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LearningPortal.Data;
using LearningPortal.Models;
using LearningPortal.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using LearningPortal.ExceptionLogger;
using Microsoft.AspNetCore.Http;
using LearningPortal.Extensions;

namespace LearningPortal
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //individual user accounts
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                //signin settings
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                //Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 2; 
                //Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                //user settings
                options.User.RequireUniqueEmail = true;
            });

            //cookies
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/Accessdenied";
                options.SlidingExpiration = true;
                // Requires using Microsoft.AspNetCore.Authentication.Cookies;
                //options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            });

            //autentizace
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = "1929513644037356";
                facebookOptions.AppSecret = "65300edf6a253707ae78f778d96378ca";
            });

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                //bez secret manageru
                googleOptions.ClientId = "312563766849-tktcvend3ge0qlig8ipniceacet2mrs7.apps.googleusercontent.com";
                googleOptions.ClientSecret = "9vgsZ1hOkdLXd4xRUXA3wO9T";

                //secret manager
                //googleOptions.ClientId = Configuration["MySecret:Authentication:Google:AppId"];
                //googleOptions.ClientSecret = Configuration["MySecret:Authentication:Google:AppSecret"];
            });

            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = "5280b2e8-2faf-4cb1-b7a9-b068b5afc2dc";
                microsoftOptions.ClientSecret = "mnumoCGIK30320/@+twBFI^";
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IUserExtensions, UserExtensions>();

            services.AddMvc();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsProduction())
            //if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        var feature = context.Features.Get<IExceptionHandlerFeature>();
                        var exception = feature.Error;
                        IExceptionLogger logException = new FileLogException();
                        logException.LogException(exception);
                        await context.Response.WriteAsync(
                            $"<b>Oops!</b> {exception.Message}");
                    });
                });

                //app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
