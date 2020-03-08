using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(QuizHut.Web.Areas.Identity.IdentityHostingStartup))]

namespace QuizHut.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}
