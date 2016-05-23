using System;
using System.Linq;
using System.Web.Security;
using Stardust.Core.Security;
using Stardust.Nucleus;
using Stardust.Particles;

namespace Stardust.Nexus.Repository
{
    public static class KeyHelper
    {
        public static EncryptionKeyContainer SharedSecret
        {
            get
            {
                return new EncryptionKeyContainer(GetKeyFromConfig());
            }
        }

        private static string GetKeyFromConfig()
        {
            var key = ConfigurationManagerHelper.GetValueOnKey("stardust.ConfigKey");
            if (key.ContainsCharacters()) return key;
            key = "defaultEncryptionKey";
            ConfigurationManagerHelper.SetValueOnKey("stardust.ConfigKey", key, true);
            return key;
        }

        public static EncryptionKeyContainer GetSecret(IConfigSet configSet)
        {
            return new EncryptionKeyContainer(GetSiteSecret(configSet));
        }

        public static string GetSiteSecret(IConfigSet configSet)
        {
            return configSet.CryptoKey.SiteEncryptionKey.Decrypt(new EncryptionKeyContainer(MachineKey.Unprotect(Convert.FromBase64String(configSet.CryptoKey.Settings.MasterEncryptionKey)).GetStringFromArray()));
        }

        public static EncryptionKeyContainer GetSecret(ConfigUser configSet)
        {
            var rep = Resolver.Activate<IRepositoryFactory>().GetRepository().Settingss.SingleOrDefault();
            return new EncryptionKeyContainer(MachineKey.Unprotect(Convert.FromBase64String(rep.MasterEncryptionKey)).GetStringFromArray());
        }
    }

}