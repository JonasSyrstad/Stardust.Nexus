using System.Collections.Generic;

namespace Stardust.Nexus.Proxy.Models
{
    public class User
    {
        public string NameId { get; set; }
        public string AccessToken { get; set; }
        public List<string> ConfigSets { get; set; }
        public bool ReadOnly { get; set; }
    }
}