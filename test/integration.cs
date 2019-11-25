using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit;
using app;
using app.Data;
using System.Net;

namespace tests
{

    public class IntegrationTest
    {
        public HttpClient _client;
        public IntegrationTest()
        {
            string wanted_path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            // TODO :  better way to get app path?
            var parent = Directory.GetParent(wanted_path).Parent;
            string env = Environment.GetEnvironmentVariable("MYSQL_DB");

            var webhost = new WebHostBuilder();
            if (!String.IsNullOrEmpty(env)) {       
                webhost.ConfigureTestServices(services => {
                    services.AddDbContext<ApplicationDbContext> (options => options.UseMySql(Environment.GetEnvironmentVariable("MYSQL_DB")));
                });
            } else {
                 IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(parent.ToString())
                    .AddJsonFile("app/appsettings.json")
                    .Build();
                 webhost.UseConfiguration(configuration);
            }
            _client = new TestServer(webhost.UseStartup<Startup>()).CreateClient();
        }

        [Fact]
        public async Task Home_Redirect_Success()        {
            
            var response = await _client.GetAsync("/");
          
            var redirectUrl = response.Headers.Location.AbsolutePath.ToString();
            Assert.Equal (HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal ("/Identity/Account/Login", redirectUrl);
        }

    }
}