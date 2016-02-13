using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ZeeSoft.Web.MVC.FI.Startup))]
namespace ZeeSoft.Web.MVC.FI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}
