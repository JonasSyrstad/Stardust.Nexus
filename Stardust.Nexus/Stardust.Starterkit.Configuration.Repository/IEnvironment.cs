using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Configuration;
using BrightstarDB.EntityFramework;
using Stardust.Core.Security;
using Stardust.Interstellar.ConfigurationReader;
using Stardust.Particles;

namespace Stardust.Nexus.Repository
{
    [Entity("Environment")]
    public interface IEnvironment
    {
        [Identifier("http://stardust.com/configuration/Environments/", KeyProperties = new[] { "ConfigSetNameId", "Name" }, KeySeparator = "-")]
        string Id { get; }
        [RegularExpression(Constants.KeyValidator, ErrorMessage = "special characters are not  allowed.")]
        string Name { get; set; }

        string ConfigSetNameId { get; set; }

        IConfigSet ConfigSet { get; set; }

        IEnvironment ParentEnvironment { get; set; }

        [InverseProperty("ParentEnvironment")]
        ICollection<IEnvironment> ChildEnvironments { get; set; }


        [InverseProperty("Environment")]
        ICollection<ISubstitutionParameter> SubstitutionParameters { get; set; }

        [InverseProperty("Environment")]
        ICollection<IEnvironmentParameter> EnvironmentParameters { get; set; }

        [RelativeOrAbsoluteIdentifier("relative")]
        [InverseProperty("Environment")]
        ICacheSettings CacheType { get; set; }

        EnvironmentConfig GetRawConfigData();
        IdentitySettings GetRawIdentityData();

        string Description { get; set; }

        string ReaderKey { get; set; }

        string ETag { get; set; }

        [DisplayName("Last updated")]
        DateTime LastPublish { get; set; }

        void SetReaderKey(string key);

        string GetReaderKey();

        [InverseProperty("Environment")]
        [PropertyType("Relative")]
        ICollection<IApiKeyContainer> ApiKeys { get; set; }

        IApiKeyContainer GetApiKeys(string serviceHostName);
    }

    [Entity("CacheSettings")]
    public interface ICacheSettings
    {
        string Id { get; }

        [Ignore]
        string CacheType { get; }

        string CacheImplementation { get; set; }

        IEnvironment Environment { get; set; }

        bool NotifyOnChange { get; set; }
        string CacheName { get; set; }

        [DisplayName("Machine names")]
        string MachineNames { get; set; }
        int Port { get; set; }
        bool Secure { get; set; }
        string PassPhrase { get; set; }

        string SecurityMode { get; set; }

        string ProtectionLevel { get; set; }

        

    }

    [Entity("ApiKeyContainer")]
    public interface IApiKeyContainer
    {
        string Id { get; }

        IServiceHostSettings ServiceHost { get; set; }

        IEnvironment Environment { get; set; }

        [PropertyType("Relative")]
        [InverseProperty("Container")]
        ICollection<IApiKey> Keys { get; set; }
    }

    [Entity("ApiKey")]
    public interface IApiKey
    {
        string Id { get; }

        string UserName { get; set; }

        string Key { get; set; }

        IApiKeyContainer Container { get; set; }

        DateTime ExpiryDate { get; set; }
    }

    public static class ApiKeyHelper
    {
        public static string CreateApiKey(this IApiKey key, string username, int validity=1)
        {
            var apiKey = $"{{{Guid.NewGuid().ToString()}}}";
            key.UserName = username;
            key.Key = apiKey.Encrypt(new EncryptionKeyContainer(key.Container.Environment.GetReaderKey()));
            key.ExpiryDate = DateTime.UtcNow.AddYears(validity);
            return apiKey;
        }
    }
}