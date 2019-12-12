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
using System.Linq;
using Joonasw.AspNetCore.SecurityHeaders;
using System.Threading.Tasks;
using Microsoft.AspNetCore.HttpOverrides;

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
            var envdb = Environment.GetEnvironmentVariable ("MYSQL_DB");
            string CnString = !String.IsNullOrEmpty (envdb) ? envdb : Configuration.GetConnectionString("test");

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
            services.AddSignalR();

            services.AddScoped<IHubRepository, HubRepository>();
            services.AddScoped<IPostsRepository, PostsRepository>();
            services.AddScoped<IRoomsRepository, RoomsRepository>();

            services.AddSingleton<IHubLogger, HubLogger>();

            // https://stackoverflow.com/questions/55733521/asp-net-core-validation-after-filters
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            
            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Filters.Add(new ModelStateValidationActionFilterAttribute());  
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            });
            //https://stackoverflow.com/questions/52954158/asp-net-core-2-1-no-http-https-redirection-in-app-engine
           services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = 
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            //https://stackoverflow.com/questions/52954158/asp-net-core-2-1-no-http-https-redirection-in-app-engine
            app.UseForwardedHeaders();
            app.Use(async (context, next) =>
            {
                if (context.Request.IsHttps || context.Request.Headers["X-Forwarded-Proto"] == Uri.UriSchemeHttps)
                {
                    await next();
                }
                else
                {
                    string queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;
                    var https = "https://" + context.Request.Host + context.Request.Path + queryString;
                    context.Response.Redirect(https);
                }
            });
            if (env.IsDevelopment())
            {
                // These will run synchronously
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
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                HstsBuilderExtensions.UseHsts(app);
            
                   app.UseCsp(csp =>
                    {
                        csp.AllowScripts
                                .FromSelf()
                                .From("https://kit.fontawesome.com");
                        csp.AllowStyles
                                .FromSelf()
                                .From("kit-free.fontawesome.com/releases/latest/css/");

                            csp.OnSendingHeader = context =>
                            {
                                context.ShouldNotSend = context.HttpContext.Request.Path.StartsWithSegments("/Identity");
                                return Task.CompletedTask;
                            };
                    });
            }

            // app.ConfigureExceptionHandler();

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
