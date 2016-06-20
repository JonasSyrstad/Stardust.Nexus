using System.IdentityModel.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Stardust.Core.Service.Web;
using Stardust.Interstellar.Rest.Service;
using Stardust.Nexus.Management.Client;
using Stardust.Particles;

namespace Stardust.Nexus.Web
{

    public class MvcApplication : HttpApplication
    {
        private static IHubContext hub;

        private static HubConnection hubConnection;

        private static IHubProxy hubClient;

        protected void Application_Start()
        {
            this.LoadBindingConfiguration().LoadMapDefinitions<MapDefinitions>();
            ServiceFactory.CreateServiceImplementation<IRegistration>();
            ServiceFactory.CreateServiceImplementation<IConfiguration>();
            ServiceFactory.FinalizeRegistration();
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        protected void Application_Error()
        {
            var ec = Server.GetLastError();
            ec.Log();
        }
    }


}
