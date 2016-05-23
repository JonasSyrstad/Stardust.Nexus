using System.Collections.Generic;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Stardust.Nexus.Proxy
{
    public class TokeAuthFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.security == null) operation.security = new List<IDictionary<string, IEnumerable<string>>>();
            operation.security.Add(new Dictionary<string, IEnumerable<string>> { { "token", new List<string> {  } } });
            operation.security.Add(new Dictionary<string, IEnumerable<string>> { { "key", new List<string> {  } } });
        }
    }
}