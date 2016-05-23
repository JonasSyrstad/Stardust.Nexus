﻿using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Stardust.Particles;

namespace Stardust.Nexus.Web.Notification
{
    [HubName("configSetHub")]
    public class ConfigSetHub : Hub
    {
        [HubMethodName("changed")]
        public void ConfigSetUpdated(string id, string environment)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ConfigSetHub>();

            context.Clients.All.changed(id, environment);
        }

        [HubMethodName("ping")]
        public void Ping(string clientId)
        {
            Logging.DebugMessage("Ping from {0}",clientId);
        }
    }
}