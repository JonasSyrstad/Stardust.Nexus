using System.Web.Mvc;
using Stardust.Interstellar;

namespace Stardust.Nexus.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Mvc.Controller"/> class.
        /// </summary>
        public AccountController(IRuntime runtime)
            : base(runtime)
        {
        }

        [HttpGet]
        public ActionResult Signin()
        {
           
            return Content("OK");
        }

    }
}