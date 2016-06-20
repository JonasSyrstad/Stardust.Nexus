using System.Web.Http;
using Stardust.Interstellar.Config;
using Stardust.Interstellar.Rest.Annotations;

namespace Stardust.Nexus.Management.Client
{
    [IRoutePrefix("v2/config")]
    public interface IConfiguration
    {
        [HttpGet]
        [Route("{configSetId}/{environment}")]
        ConfigurationSet GetConfiguration(string configSetId, string environment);
    }

    [IRoutePrefix("v2/management")]
    public interface IRegistration
    {
        [HttpPut]
        [Route("{configSet}/initialize/environment")]
        void InitializeEnvironment([In(InclutionTypes.Path)]string configSet,[In(InclutionTypes.Body)]EnvironmentSettings settings);

        [HttpDelete]
        [Route("delete({configSet},{environment})")]
        void DeleteEnvironment([In(InclutionTypes.Path)] string configSet, [In(InclutionTypes.Path)] string environment);

        [HttpDelete]
        [Route("publish({configSet},{environment})")]
        void PublishEnvironment([In(InclutionTypes.Path)] string configSet, [In(InclutionTypes.Path)] string environment);

        [HttpPut]
        [Route("{configSet}/initialize/datcenter")]
        ConfigurationSettings InitializeDatacenter([In(InclutionTypes.Path)]string configSet, [In(InclutionTypes.Body)]ConfigurationSettings settings);

        [HttpPost]
        [Route("{configSet}/publish/datcenter")]
        ConfigurationSettings PublishDatacenter([In(InclutionTypes.Path)] string configSet, [In(InclutionTypes.Body)] ConfigurationSettings settings);

        [HttpPost]
        [Route("{configSet}/property")]
        void SetProperty([In(InclutionTypes.Path)] string configSet, [In(InclutionTypes.Body)]PropertyRequest property);
    }
}