using Logistics.DataAccessLayer;
using Logistics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Logistics.Controllers
{
    public class AboutController : Controller
    {
        private readonly AppDbContext _db;
        public AboutController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            AboutVM aboutVM = new AboutVM
            {
                About = await _db.Abouts.
                        Where(x=>!x.IsDeleted && x.Language.Culture == CultureInfo.CurrentCulture.Name)
                        .FirstOrDefaultAsync(),
                Priority = await _db.Priorities
                          .Where(x=>!x.IsDeleted && x.Language.Culture == CultureInfo.CurrentCulture.Name)
                          .FirstOrDefaultAsync(),
                PriorityCards = await _db.PriorityCards
                          .Where(x=>!x.IsDeleted && x.Language.Culture == CultureInfo.CurrentCulture.Name)
                          .Take(4)
                          .ToListAsync()
                
            };
            return View(aboutVM);
        }
    }
}
