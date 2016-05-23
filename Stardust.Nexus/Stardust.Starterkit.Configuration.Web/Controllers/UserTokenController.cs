using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Stardust.Nexus.Business;
using Stardust.Nexus.Repository;

namespace Stardust.Nexus.Web.Controllers
{
    [Authorize]
    public class UserTokenController : ApiController
    {
        private IConfigSetTask reader;

        private IUserFacade userFacade;

        public UserTokenController(IConfigSetTask reader, IUserFacade userFacade)
        {
            this.reader = reader;
            this.userFacade = userFacade;
        }
        [HttpGet]
        public HttpResponseMessage GetUser(string id)
        {
            try
            {
                var user = userFacade.GetUser(id);
                return Request.CreateResponse(user != null ? new
                                                                 {
                                                                     user.NameId,
                                                                     user.AccessToken,
                                                                     ConfigSets = user.AdministratorType == AdministratorTypes.SystemAdmin ? reader.GetAllConfigSetNames() : user.ConfigSet.Select(c => c.Id).ToList(),
                                                                     ReadOnly=user.AdministratorType!=AdministratorTypes.ConfigReader
                                                                 } : CreateDeletedResponse(id));
            }
            catch (NullReferenceException)
            {
                return Request.CreateResponse(CreateDeletedResponse(id));
            }
        }

        private static object CreateDeletedResponse(string id)
        {
            return new { NameId = id, AccessToken = "deleted", ConfigSets = new List<string>() };
        }
    }
}