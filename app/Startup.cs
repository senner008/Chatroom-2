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
using app.Repositories;
using app.Controllers;

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

        //    services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("chatroom"));

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
            services.AddScoped<IDisposable, UserManager<ApplicationUser>>();
            services.AddScoped<HttpContextAccessor>();
            services.AddScoped<IHubRepository, HubRepository>();
            services.AddScoped<IPostsRepository, PostsRepository>();
            services.AddScoped<IRoomsRepository, RoomsRepository>();



            services.AddSingleton<IHubLogger, HubLogger>();
            
            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(RoleManager<IdentityRole> roleManager, IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                // These will run synchronously when without await 
                var pass = Configuration.GetSection ("Passwords").GetSection ("adminpass").Value;
                SeedData.SeedApplicationUsers (userManager, "alpha@mail.com", "alpha", pass, roleManager);
                SeedData.SeedApplicationUsers (userManager, "beta@mail.com", "beta", pass, roleManager);
                SeedData.SeedApplicationUsers (userManager, "gamma@mail.com", "gamma", pass, roleManager);

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
                endpoints.MapHub<ChatHub>("/hub");
            });
        }
    }
}
