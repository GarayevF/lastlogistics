using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.Areas.Admin.Controllers
{
    [Area("Admin")]
	//[Authorize(Roles = "SuperAdmin")]
	public class DashboardController : Controller
    {
        #region Index
        public IActionResult Index()
        {
            return View();
        }
        #endregion
    }
}
