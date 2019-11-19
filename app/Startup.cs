using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using app.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalRChat.Hubs;
using Microsoft.AspNetCore.Http;
using app.Models;
using Microsoft.AspNetCore.Mvc;

namespace app
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

            Boolean isProduction = !String.IsNullOrEmpty (Environment.GetEnvironmentVariable ("HEROKU_PRODUCTION"));
            string CnString = isProduction ? Environment.GetEnvironmentVariable ("MYSQL_DB") : Configuration.GetConnectionString("CodeToShowDb");

            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(CnString));

            services.AddDefaultIdentity<ApplicationUser> (options => {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 15;
            })
            .AddRoles<IdentityRole> ()
            .AddEntityFrameworkStores<ApplicationDbContext> ();

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddSignalR();
            services.AddScoped<UserManager<ApplicationUser>>();
            services.AddScoped<HttpContextAccessor>();
            services.AddScoped<MessageHandler>();
            services.AddSingleton<HubLogger>();
            


            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                var pass = Configuration.GetSection ("Passwords").GetSection ("adminpass").Value;
                SeedData.SeedApplicationUsers (userManager, "alpha@mail.com", "alpha", pass);
                SeedData.SeedApplicationUsers (userManager, "beta@mail.com", "beta", pass);
                SeedData.SeedApplicationUsers (userManager, "gamma@mail.com", "gamma", pass);

                SeedData.SeedApplicationRooms(context, userManager);
                SeedData.SeedApplicationPosts(context, userManager, Configuration.GetConnectionString("CodeToShowDb"));

                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
            }
            app.UseHttpsRedirection();
             app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("chatHub");
            });
        }
    }
}
