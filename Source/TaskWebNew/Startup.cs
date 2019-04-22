using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TaskWebNew.Startup))]
namespace TaskWebNew
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
