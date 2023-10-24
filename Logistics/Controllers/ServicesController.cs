using Logistics.DataAccessLayer;
using Logistics.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Logistics.Controllers
{
    public class ServicesController : Controller
    {
        private readonly AppDbContext _db;
        public ServicesController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null) return BadRequest();
            Service service = await _db.Services
                              .Where(x => !x.IsDeleted && x.Language.Culture == CultureInfo.CurrentCulture.Name)
                              .Include(x=>x.ServiceServiceSections)
                              .ThenInclude(x=>x.ServiceSection)
                              .FirstOrDefaultAsync(a => a.Id == id);
            if (service == null) return NotFound();
            return View(service);
        }
    }
}
