using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stardust.Core.Security;
using Stardust.Interstellar;
using Stardust.Particles;
using Stardust.Starterkit.Configuration.Business;
using Stardust.Starterkit.Configuration.Repository;

namespace Stardust.Starterkit.Configuration.Web.Controllers
{
    [Authorize]
    public class SettingsController : BaseController
    {
        private readonly ISettingsFacade reader;

        // GET: Settings
        public ActionResult Index()
        {
            if (CurrentUser.AdministratorType != AdministratorTypes.SystemAdmin) throw new UnauthorizedAccessException("Unauthorized");
            var settings = reader.GetSettings();
            return View(settings);
        }

        [HttpPost]
        public ActionResult Index(Settings model)
        {
            if (CurrentUser.AdministratorType != AdministratorTypes.SystemAdmin) throw new UnauthorizedAccessException("Unauthorized");
            reader.RegenerateMasterKey();
            var settings = reader.GetSettings();
            return View(settings);
        }

        public ActionResult Key(string id)
        {
            var keyHolder = reader.GetSettings().SiteEncryptions.SingleOrDefault(s => s.Site.Id == id);
            ViewBag.Key = keyHolder.SiteEncryptionKey.Decrypt(KeyHelper.GetSecret(keyHolder.Site));
            return View();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Mvc.Controller"/> class.
        /// </summary>
        public SettingsController(IRuntime runtime, ISettingsFacade reader)
            : base(runtime)
        {
            this.reader = reader;
        }
    }
}