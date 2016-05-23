using System.Collections.Generic;

namespace Stardust.Nexus.Proxy.Models
{
    public class ConsolidatedConfigWrapperFile
    {
        public Dictionary<string, ConfigWrapper> ConfigWrappers { get; set; }

        public Dictionary<string, User> Users { get; set; } 
    }
}