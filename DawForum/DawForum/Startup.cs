using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DawForum.Startup))]
namespace DawForum
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
