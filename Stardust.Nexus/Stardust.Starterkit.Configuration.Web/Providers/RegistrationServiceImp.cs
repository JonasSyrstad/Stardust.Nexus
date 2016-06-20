using System;
using System.Linq;
using Stardust.Interstellar.Config;
using Stardust.Nexus.Business;
using Stardust.Nexus.Management.Client;
using Stardust.Nexus.Repository;
using Stardust.Wormhole;
using ConfigurationSettings = Stardust.Nexus.Management.Client.ConfigurationSettings;
using VariableTypes = Stardust.Nexus.Web.Models.VariableTypes;

namespace Stardust.Nexus.Web.Providers
{
    public class ConfigReaderServiceImp:IConfiguration
    {
        private IEnvironmentTasks environmentTasks;

        private IConfigSetTask configSetTasks;

        public ConfigReaderServiceImp(IEnvironmentTasks environmentTasks, IConfigSetTask configSetTasks)
        {
            this.environmentTasks = environmentTasks;
            this.configSetTasks = configSetTasks;
        }

        public ConfigurationSet GetConfiguration(string configSetId, string environment)
        {
            return configSetTasks.GetConfigSetData(configSetId, environment).Map().To<ConfigurationSet>();
        }
    }

    public class RegistrationServiceImp : IRegistration
    {
        private IEnvironmentTasks environmentTasks;

        private IConfigSetTask configSetTasks;

        public RegistrationServiceImp(IEnvironmentTasks environmentTasks, IConfigSetTask configSetTasks
           )
        {
            this.environmentTasks = environmentTasks;
            this.configSetTasks = configSetTasks;
        }
        public void InitializeEnvironment(string configSet, EnvironmentSettings settings)
        {
            
        }

        public void DeleteEnvironment(string configSet, string environment)
        {
            throw new NotImplementedException();
        }

        public void PublishEnvironment(string configSet, string environment)
        {
            throw new NotImplementedException();
        }

        public ConfigurationSettings InitializeDatacenter(string configSet, ConfigurationSettings settings)
        {
            var result=configSetTasks.InitializeDatacenter(configSet, settings.Map().To<Repository.ConfigurationSettings>());
            return result.Map().To<ConfigurationSettings>();
        }

        public ConfigurationSettings PublishDatacenter(string configSet, Management.Client.ConfigurationSettings settings)
        {
            var result = configSetTasks.PublishDatacenter(configSet, settings.Map().To<Repository.ConfigurationSettings>());
            return result.Map().To<ConfigurationSettings>();
        }

        public void SetProperty(string id, PropertyRequest property)
        {
            var settings = property.Map().To<Models.PropertyRequest>();
            var env = environmentTasks.GetEnvironment(string.Format("{0}-{1}", id, settings.Environment));
            switch (settings.Type)
            {
                case VariableTypes.Environmental:
                    SetEnvironmentVariable(env, settings.PropertyName, settings.Value, settings.IsSecure);
                    break;
                case VariableTypes.ServiceHost:
                case VariableTypes.ServiceHostEnvironmental:
                    var host = configSetTasks.GetServiceHost(string.Format("{0}-{1}", id, settings.ParentContainer));
                    SetHostParameter(configSetTasks, settings, host);
                    if (settings.Type == VariableTypes.ServiceHostEnvironmental)
                    {
                        var envKey = GetEnvironmentSubstitutionKey(settings);
                        SetEnvironmentSubstitutionVariable(env, envKey, settings.Value, settings.IsSecure);
                    }
                    break;
                case VariableTypes.Service:
                case VariableTypes.ServiceEnvironmental:
                    SetEnpointParameter(id,settings);
                    if (settings.Type == VariableTypes.ServiceHostEnvironmental)
                    {
                        var envKey = GetEnvironmentSubstitutionKey(settings);
                        SetEnvironmentSubstitutionVariable(env, envKey, settings.Value, settings.IsSecure);
                    }
                    break;
            }
        }

        private void SetEnpointParameter(string id,Models.PropertyRequest settings)
        {
            var endpoint = configSetTasks.GetEndpoint(string.Format("{0}-{1}-{2}", id, settings.ParentContainer, settings.SubContainer));
            var item = endpoint.Parameters.SingleOrDefault(p => p.Name == settings.PropertyName);
            if (item == null)
            {
                configSetTasks.CreateEndpointParameter(
                    settings.ParentContainer,
                    settings.PropertyName,
                    settings.Type == Models.VariableTypes.ServiceHostEnvironmental ? settings.ParentFormatString : settings.Value,
                    settings.Type == Models.VariableTypes.ServiceHostEnvironmental);
            }
            else
            {

                item.ConfigurableForEachEnvironment = settings.Type == Models.VariableTypes.ServiceHostEnvironmental;
                item.ItemValue =
                    settings.Type == Models.VariableTypes.ServiceHostEnvironmental ? settings.ParentFormatString : settings.Value;
                configSetTasks.UpdateEndpointParameter(item);
            }
        }

        private string GetEnpointParameterKey(string id,Models.PropertyRequest settings)
        {
            return string.Format("{0}-{1}-{2}-{3}", id, settings.ParentContainer, settings.SubContainer, settings.PropertyName);
        }

        private string GetEnpointKey(string id,Models.PropertyRequest settings)
        {
            return string.Format("{0}-{1}-{2}", id, settings.ParentContainer, settings.SubContainer, settings.PropertyName);
        }

        private void SetHostParameter(IConfigSetTask reader, Models.PropertyRequest settings, IServiceHostSettings host)
        {
            var item = host.Parameters.SingleOrDefault(p => p.Name == settings.PropertyName);
            if (item == null)
            {
                reader.CreateServiceHostParameter(
                    host,
                    settings.PropertyName,
                    settings.IsSecure,
                    settings.Type == Models.VariableTypes.ServiceHostEnvironmental ? settings.ParentFormatString : settings.Value,
                    settings.Type == Models.VariableTypes.ServiceHostEnvironmental);
            }
            else
            {
                item.IsSecureString = settings.IsSecure;
                item.IsEnvironmental = settings.Type == Models.VariableTypes.ServiceHostEnvironmental;
                item.SetValue(
                    settings.Type == VariableTypes.ServiceHostEnvironmental ? settings.ParentFormatString : settings.Value);
                reader.UpdateHostParameter(item);
            }
        }

        private void SetEnvironmentVariable(IEnvironment env, string name, string value, bool isSecure)
        {
            var item = env.EnvironmentParameters.SingleOrDefault(p => p.Name == name);
            if (item == null)
                environmentTasks.CreatEnvironmentParameter(env, name, value, isSecure);
            else
            {
                item.IsSecureString = isSecure;
                item.SetValue(value);
                environmentTasks.UpdateEnvironmentParameter(item);
            }
        }


        private void SetEnvironmentSubstitutionVariable(IEnvironment env, string name, string value, bool isSecure)
        {
            var item = env.SubstitutionParameters.SingleOrDefault(p => p.Name == name);
            if (item == null) environmentTasks.CreateSubstitutionParameter(env, name, value);
            else
            {
                item.ItemValue = value;
                environmentTasks.UpdateSubstitutionParameter(item);
            }
        }
        private string GetEnvironmentSubstitutionKey(Models.PropertyRequest settings)
        {
            return string.Format("{0}_{1}", settings.ParentContainer, settings.PropertyName);
        }
    }
}