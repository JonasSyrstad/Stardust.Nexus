using System.Net.Http;

namespace Stardust.Nexus.Proxy.Controllers.api
{
    public interface IConfigReaderController
    {
        HttpResponseMessage Get(string id, string env = null, string updKey = null);
    }
}