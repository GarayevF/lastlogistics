using Logistics.DataAccessLayer;
using Logistics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Logistics.Controllers
{
    public class IncotermsController : Controller
    {
        private readonly AppDbContext _db;
        public IncotermsController(AppDbContext db) 
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            IncotermsVM incotermsVM = new IncotermsVM
            {
                Incoterms = await _db.Incoterms.Where(x=>!x.IsDeleted && x.Language.Culture == CultureInfo.CurrentCulture.Name).FirstOrDefaultAsync(),
                IncotermsSection = await _db.IncotermsSections.Where(x=>!x.IsDeleted && x.Language.Culture == CultureInfo.CurrentCulture.Name).ToListAsync(),
            };
            return View(incotermsVM);
        }
    }
}
