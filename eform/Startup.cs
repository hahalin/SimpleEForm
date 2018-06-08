using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eform.Startup))]
namespace eform
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
