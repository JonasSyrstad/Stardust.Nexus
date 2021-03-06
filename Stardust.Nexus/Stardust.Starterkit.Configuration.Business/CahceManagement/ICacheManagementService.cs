﻿using System;
using Stardust.Interstellar;

namespace Stardust.Nexus.Business.CahceManagement
{
    public interface ICacheManagementService : IRuntimeTask
    {
        bool TryUpdateCache(string configSet, string environment);

        void RegisterRealtimeNotificationService(Action<string, string> action);

        void NotifyUserChange(string id);
    }
}
