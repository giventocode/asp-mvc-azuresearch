using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(aspnet_mvc_azuresearch.Startup))]
namespace aspnet_mvc_azuresearch
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
