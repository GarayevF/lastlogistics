using Logistics.DataAccessLayer;
using Logistics.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Logistics.Controllers
{
    public class ContainerTypesController : Controller
    {
        private readonly AppDbContext _db;
        public ContainerTypesController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            List<ContainerType> containerTypes = await _db.ContainerTypes.Where(x => !x.IsDeleted && x.Language.Culture == CultureInfo.CurrentCulture.Name).ToListAsync();
            return View(containerTypes);
        }
    }
}
