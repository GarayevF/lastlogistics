using Microsoft.AspNetCore.Mvc;

namespace Logistics.Controllers
{
    public class ContactsController : Controller
    {
        public IActionResult BakuOffice()
        {
            return View();
        }
        public IActionResult ChineseOffice()
        {
            return View();
        }
    }
}
