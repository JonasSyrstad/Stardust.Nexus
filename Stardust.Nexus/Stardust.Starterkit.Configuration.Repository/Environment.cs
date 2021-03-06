﻿using System;
using System.Collections.Generic;
using System.Linq;
using Stardust.Interstellar.ConfigurationReader;
using Stardust.Particles;
using Stardust.Wormhole;

namespace Stardust.Nexus.Repository
{
    public partial class Environment
    {
        public override string ToString()
        {
            return Id;
        }

        public EnvironmentConfig GetRawConfigData()
        {
            return new EnvironmentConfig
            {
                EnvironmentName = Name,
                Parameters = (from p in EnvironmentParameters select p.GetRawConfigData()).ToList(),
                Cache = CacheType.Map().To<Interstellar.ConfigurationReader.CacheSettings>(),
                ReaderKey = ReaderKey
            };
        }

        public IdentitySettings GetRawIdentityData()
        {
            return new IdentitySettings
            {
                Audiences = new List<string> {GetValue("Realm")},
                IssuerName = GetValue("IssuerName"),
                CertificateValidationMode = GetValue("CertificateValidationMode"),
                EnforceCertificateValidation = GetValue("EnforceCertificateValidation").Equals("true",StringComparison.CurrentCultureIgnoreCase),
                IssuerAddress = GetValue("StsAddress"),
                Thumbprint = GetValue("Thumbprint"),
                Realm = GetValue("Realm"),
                RequireHttps = GetValue("RequireHttps").Equals("true", StringComparison.CurrentCultureIgnoreCase),
                Certificate = EnvironmentParameters.Where( p=>p.Name=="Certificate").Select(p=>p.Value).SingleOrDefault(),
                MetadataUrl = GetValue("IssuerMetadataAddress")
            }; 
        }

        public void SetReaderKey(string key)
        {
            ReaderKey = key.Encrypt(KeyHelper.GetSecret(this.ConfigSet));
        }

        public string GetReaderKey()
        {
            return ReaderKey.Decrypt(KeyHelper.GetSecret(this.ConfigSet));
        }

        public IApiKeyContainer GetApiKeys(string serviceHostName)
        {
            throw new NotImplementedException();
        }

        private string GetValue(string key)
        {
            return SubstitutionParameters.Single(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase)).Value;
        }
    }
}