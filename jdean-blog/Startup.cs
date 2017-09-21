using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(jdean_blog.Startup))]
namespace jdean_blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
