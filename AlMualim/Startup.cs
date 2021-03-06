using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using AlMualim.Data;
using AlMualim.Services;

namespace AlMualim
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
            services.AddDbContext<AlMualimDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AlMualimDbContext")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AlMualimDbContext>();
            services.AddControllersWithViews();

            services.AddSingleton<IAzureBlobService, AzureBlobService>();
            services.AddSingleton<IEmailGenerator, EmailGenerator>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IEmailQueue, EmailQueue>();
            services.AddTransient<ISubscriptionService, SubscriptionService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "slug",
                    pattern: "Surah/Notes/{slug?}",
                    defaults: new { controller = "Surah", action = "Notes" });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapGet("/Identity/Account/Register", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true)));
                endpoints.MapPost("/Identity/Account/Register", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true)));
                endpoints.MapGet("/Identity/Account/ResendEmailConfirmation", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true)));
                endpoints.MapPost("/Identity/Account/ResendEmailConfirmation", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true)));
                endpoints.MapRazorPages();
            });
        }
    }
}
