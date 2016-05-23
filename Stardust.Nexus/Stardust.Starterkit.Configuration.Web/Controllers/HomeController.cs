using System.Web.Mvc;
using System.Web.Security;
using Stardust.Interstellar;
using Stardust.Nexus.Business;

namespace Stardust.Nexus.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private IEnvironmentTasks environmentTasks;

        private IConfigSetTask reader;

        public HomeController(IRuntime runtime,IEnvironmentTasks environmentTasks, IConfigSetTask reader)
            : base(runtime)
        {
            this.environmentTasks = environmentTasks;
            this.reader = reader;
        }
        public ActionResult Index()
        {
           var providers= Roles.Providers;
                        
            return View(reader.GetAllConfitSets());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}