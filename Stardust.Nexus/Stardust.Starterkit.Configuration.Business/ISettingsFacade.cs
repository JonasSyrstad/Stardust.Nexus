using System;
using System.Linq;
using System.Web.Security;
using Stardust.Core.Security;
using Stardust.Interstellar;
using Stardust.Particles;
using Stardust.Particles.Xml;
using Stardust.Starterkit.Configuration.Repository;

namespace Stardust.Starterkit.Configuration.Business
{
    public interface ISettingsFacade
    {
        ISettings GetSettings();

        void RegenerateMasterKey();
    }

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
                var oldSiteEnc = siteEncryptionse.SiteEncryptionKey.Decrypt(new EncryptionKeyContainer(oldKey));
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
                ReencryptEnvironments(siteEncryptionse, oldKey, newKey, newSiteEnc, oldSiteEnc);
                ReencryptServiceHostParameters(siteEncryptionse, oldSiteEnc, newSiteEnc);
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
            settings.MasterEncryptionKey = Convert.ToBase64String(MachineKey.Protect(newKey.GetByteArray()));
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