using Logistics.DataAccessLayer;
using Logistics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Logistics.Controllers
{
    public class OurCustomersController : Controller
    {
        private readonly AppDbContext _db;
        public OurCustomersController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            OurCustomersVM ourCustomersVM = new OurCustomersVM
            {
                Statistics = await _db.Statistics.Where(x => !x.IsDeleted && x.Language.Culture == CultureInfo.CurrentCulture.Name).Take(4).ToListAsync(),
                Customers = await _db.Customers.Where(x=>!x.IsDeleted).ToListAsync(),
            };
            return View(ourCustomersVM);
        }
    }
}
