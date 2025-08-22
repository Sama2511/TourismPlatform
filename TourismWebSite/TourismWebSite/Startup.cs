using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TourismWebSite.Startup))]
namespace TourismWebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
