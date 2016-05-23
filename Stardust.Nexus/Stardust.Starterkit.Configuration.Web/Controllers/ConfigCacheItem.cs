using Stardust.Interstellar.ConfigurationReader;

namespace Stardust.Nexus.Web.Controllers
{
    internal class ConfigCacheItem
    {
        public string ETag { get; set; }

        public ConfigurationSet ConfigSet { get; set; }
    }
}