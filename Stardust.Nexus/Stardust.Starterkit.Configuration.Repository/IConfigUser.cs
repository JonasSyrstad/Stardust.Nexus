﻿using System.Collections.Generic;
using System.ComponentModel;
using BrightstarDB.EntityFramework;
using Stardust.Particles;

namespace Stardust.Nexus.Repository
{
    [Entity("ConfigUser")]
    public interface IConfigUser
    {
        [Identifier("http://stardust.com/configuration/Users", KeyProperties = new[] { "NameId" })]
        string Id { get; }

        string NameId { get; set; }
        
        string Email { get; set; }       

        [InverseProperty("Administrators")]
        ICollection<IConfigSet> ConfigSet { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        [DisplayName("Role")]
        AdministratorTypes AdministratorType { get; set; }

        string AccessToken { get; set; }

        void SetAccessToken(string token);

        string GetAccessToken();
    }

    public partial class ConfigUser
    {
        public override string ToString()
        {
            return string.Format("{0}, {1}", LastName, FirstName);
        }

        public void SetAccessToken(string key)
        {
            AccessToken = key.Encrypt(KeyHelper.GetSecret(this));
        }

        public string GetAccessToken()
        {
            return AccessToken.Decrypt(KeyHelper.GetSecret(this));
        }
    }
}