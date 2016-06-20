using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Stardust.Nexus.Management.Client
{
    public class ConfigurationSettings
    {
        public string DataCenterName { get; set; }

        public string Environment { get; set; }

        public string ServiceRootUri { get; set; }

        public string DataCenterServiceHostName { get; set; }

        public string DatabaseUriKeyName { get; set; }

        public string ServiceBusUriKeyName { get; set; }

        public string ReplicationBusKeyName { get; set; }

        public string DatabaseKeyName { get; set; }

        public string ServiceBusKeyName { get; set; }

        public string ReplicationUriBusKeyName { get; set; }

        public string ReplicationBusUri { get; set; }

        public string ServiceBusUri { get; set; }

        public string DatabaseUri { get; set; }

        public string DatabaseAccessKey { get; set; }

        public string ReplicationBusAccessKey { get; set; }

        public string ServiceBusAccessKey { get; set; }

        public List<string> DataCenterList { get; set; }

        public string ReplicationBusNamespaceFormat { get; set; }
        public string ServiceBusNamespaceFormat { get; set; }
        public string DatabaseUriFormat { get; set; }
    }
}
