using Stardust.Nexus.Repository;
using Stardust.Wormhole;

namespace Stardust.Nexus.Web
{
    public class MapDefinitions : IMappingDefinition
    {
        public void Register()
        {
            MapFactory.CreateMapRule<ICacheSettings, ICacheSettings>().GetRule().RemoveMapping("Environment");
        }
    }
}