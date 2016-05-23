using Stardust.Nexus.Repository;

namespace Stardust.Nexus.Business
{
    public interface ISettingsFacade
    {
        ISettings GetSettings();

        void RegenerateMasterKey();

        void RegenerateSiteKey(string siteId);
    }
}