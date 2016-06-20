using System;
using System.Collections.Generic;

namespace Stardust.Nexus.Management.Client
{
    [Serializable]
    public class EnvironmentSettings
    {
        public string Environment { get; set; }

        public string FederationServerUrl { get; set; }

        public string FederationNamespace { get; set; }

        public string Thumbprint { get; set; }

        public string WebApplicationUrl { get; set; }

        public Dictionary<string, string> ServiceHostRootUrl { get; set; }

        public string PassiveFederationEndpoint { get; set; }

        public Dictionary<string, Dictionary<string, string>> OtherSettings { get; set; }

        public string DelegationUserName { get; set; }

        public string DelegationPassword { get; set; }
    }
}