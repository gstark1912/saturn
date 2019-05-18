using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(App.Startup))]
namespace App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Authentication
            ConfigureAuth(app);

            // Hangfire
            ConfigureHangfire(app);
        }
    }
}
