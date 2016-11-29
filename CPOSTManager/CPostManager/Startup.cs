using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup), "Configuration")]

public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        CPostManager.CPostManager.Startup.ConfigureSignalR(app);
    }
}

