using Owin;

namespace CPostManager.CPostManager
{
    public static class Startup
    {
        public static void ConfigureSignalR(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
