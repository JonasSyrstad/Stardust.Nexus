using Stardust.Interstellar;
using Stardust.Interstellar.ConfigurationReader;
using Stardust.Nexus.Repository;

namespace Stardust.Nexus.Business.CahceManagement
{
    /// <summary>
    /// Handles communication with the cache server in the client applications
    /// </summary>
    public interface ICacheManagementWrapper : IRuntimeTask
    {
        bool UpdateCache(IEnvironment environmentSettings, ConfigurationSet raw);
    }
}