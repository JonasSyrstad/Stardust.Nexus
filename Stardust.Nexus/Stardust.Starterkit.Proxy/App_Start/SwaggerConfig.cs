using System.ServiceModel;
using System.Web.Http;
using Stardust.Nexus.Proxy;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Stardust.Nexus.Proxy
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {

                        c.Schemes(new[] { "https" });


                        c.SingleApiVersion("v1", "Stardust.Starterkit.Proxy");

                        c.ApiKey("key").Description("API token key").Name("key").In("header");
                        c.ApiKey("token").Description("token")
                        .Name("Authorization")
                        .In("header");
                        
                        c.IgnoreObsoleteActions();
                        c.IgnoreObsoleteProperties();
                        c.MapType<TransferMode>(() => new Schema { type = "integer", format = "int32" });

                        c.OperationFilter<TokeAuthFilter>();

                    })
                .EnableSwaggerUi(c =>
                    {

                    });
        }
    }
}
