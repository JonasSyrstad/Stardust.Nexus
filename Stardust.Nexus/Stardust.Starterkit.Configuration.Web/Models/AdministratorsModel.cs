using System.Collections.Generic;
using Stardust.Nexus.Repository;

namespace Stardust.Nexus.Web.Models
{
    public class AdministratorsModel
    {
        public List<IConfigUser> CurrentAdministrators { get; set; }
        public List<IConfigUser> AvaliableUsers { get; set; }

        public string[] PostedUserIds { get; set; }
    }
}