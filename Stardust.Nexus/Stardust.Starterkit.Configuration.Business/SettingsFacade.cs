using System;
using System.Linq;
using System.Web.Security;
using Stardust.Core.Security;
using Stardust.Interstellar;
using Stardust.Nexus.Repository;
using Stardust.Particles;

namespace Stardust.Nexus.Business
{
    public class SettingsFacade : ISettingsFacade
    {
        private readonly IRuntime runtime;

        private readonly IRepositoryFactory repositoryFactory;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public SettingsFacade(IRuntime runtime, IRepositoryFactory repositoryFactory)
        {
            this.runtime = runtime;
            this.repositoryFactory = repositoryFactory;
        }

        public ISettings GetSettings()
        {
            var settings = repositoryFactory.GetRepository().Settingss.FirstOrDefault();
            return settings;
        }

        public void RegenerateMasterKey()
        {
            var settings = GetSettings();
            var newKey = UniqueIdGenerator.CreateNewId(26);
            var oldKey = MachineKey.Unprotect(Convert.FromBase64String(settings.MasterEncryptionKey)).GetStringFromArray();
            foreach (var siteEncryptionse in settings.SiteEncryptions)
            {
                ReencryptSite(siteEncryptionse, oldKey, newKey);
            }
            settings.MasterEncryptionKey = Convert.ToBase64String(MachineKey.Protect(newKey.GetByteArray()));
            repositoryFactory.GetRepository().SaveChanges();
        }

        private void ReencryptSite(ISiteEncryptions siteEncryption, string oldKey, string newKey)
        {
            var oldSiteEnc = siteEncryption.SiteEncryptionKey.Decrypt(new EncryptionKeyContainer(oldKey));
            var newSiteEnc = UniqueIdGenerator.CreateNewId(26);
            //if (siteEncryptionse.Site.ReaderKey.ContainsCharacters())
            //{

            //    try
            //    {
            //        var configKey = siteEncryptionse.Site.ReaderKey.Decrypt(new EncryptionKeyContainer(oldSiteEnc));
            //        siteEncryptionse.Site.ReaderKey = configKey.Encrypt(new EncryptionKeyContainer(newSiteEnc));
            //    }
            //    catch (Exception ex)
            //    {
            //        ex.Log();
            //    }
            //}
            ReencryptEnvironments(siteEncryption, oldKey, newKey, newSiteEnc, oldSiteEnc);
            ReencryptServiceHostParameters(siteEncryption, oldSiteEnc, newSiteEnc);
            foreach (var configUser in repositoryFactory.GetRepository().ConfigUsers)
            {
                if (configUser.AccessToken.ContainsCharacters())
                {
                    string oldTOken;
                    try
                    {
                        oldTOken = configUser.AccessToken.Decrypt(new EncryptionKeyContainer(oldKey));
                    }
                    catch
                    {
                        oldTOken = configUser.AccessToken.Decrypt(KeyHelper.SharedSecret);
                    }
                    configUser.AccessToken = oldTOken.Encrypt(new EncryptionKeyContainer(newKey));
                }
            }
        }

        public void RegenerateSiteKey(string siteId)
        {
            var settings=GetSettings();
            var site = settings.SiteEncryptions.SingleOrDefault(s => s.Site.Id == siteId);
            var key=MachineKey.Unprotect(Convert.FromBase64String(settings.MasterEncryptionKey)).GetStringFromArray();
            ReencryptSite(site,key,key);
            repositoryFactory.GetRepository().SaveChanges();
        }

        private static void ReencryptServiceHostParameters(ISiteEncryptions siteEncryptionse, string oldSiteEnc, string newSiteEnc)
        {
            foreach (var serviceHostSettingse in siteEncryptionse.Site.ServiceHosts)
            {
                foreach (var parameter in serviceHostSettingse.Parameters)
                {
                    if (parameter.IsSecureString && parameter.ItemValue.ContainsCharacters())
                    {
                        var val = parameter.ItemValue.Decrypt(new EncryptionKeyContainer(oldSiteEnc));
                        parameter.ItemValue = val.Encrypt(new EncryptionKeyContainer(newSiteEnc));
                        parameter.BinaryValue = parameter.ItemValue.GetByteArray();
                    }
                }
            }
        }

        private static void ReencryptEnvironments(ISiteEncryptions siteEncryptionse, string oldKey, string newKey, string newSiteEnc, string oldSiteEnc)
        {

            siteEncryptionse.SiteEncryptionKey = newSiteEnc.Encrypt(new EncryptionKeyContainer(newKey));
            foreach (var env in siteEncryptionse.Site.Environments)
            {
                if (env.ReaderKey.ContainsCharacters())
                {

                    var key = env.ReaderKey.Decrypt(new EncryptionKeyContainer(oldSiteEnc));
                    env.ReaderKey = key.Encrypt(new EncryptionKeyContainer(newSiteEnc));
                }
                foreach (var parameter in env.EnvironmentParameters)
                {
                    if (parameter.IsSecureString && parameter.ItemValue.ContainsCharacters())
                    {
                        var val = parameter.ItemValue.Decrypt(new EncryptionKeyContainer(oldSiteEnc));
                        parameter.ItemValue = val.Encrypt(new EncryptionKeyContainer(newSiteEnc));
                        parameter.BinaryValue = parameter.ItemValue.GetByteArray();
                    }
                }
                foreach (var parameter in env.SubstitutionParameters)
                {
                    if (parameter.IsSecure && parameter.ItemValue.ContainsCharacters())
                    {
                        var val = parameter.ItemValue.Decrypt(new EncryptionKeyContainer(oldSiteEnc));
                        parameter.ItemValue = val.Encrypt(new EncryptionKeyContainer(newSiteEnc));
                    }
                }
            }
        }
    }
}