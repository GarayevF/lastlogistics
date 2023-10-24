using ApplianceRepair.Extensions;
using ApplianceRepair.Helpers;
using Logistics.DataAccessLayer;
using Logistics.Models;
using Logistics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ColorsController : Controller
    {
        private readonly AppDbContext _db;
        public ColorsController(AppDbContext db)
        {
            _db = db;
        }

        #region Index
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Color> query = _db.Colors.Where(x=>!x.IsDeleted);
            return View(PageNatedList<Color>.Create(query, pageIndex, 5, 5));
        }
        #endregion

        #region Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Color color)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            color.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _db.Colors.AddAsync(color);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(int? id)
        {

            if (id == null) return BadRequest();

            Color color = await _db.Colors.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (color == null) return NotFound();

            return View(color);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Color color)
        {
            if (!ModelState.IsValid) return View(color);

            if (id == null) return BadRequest();

            if (id != color.Id) return BadRequest();

            Color dbColor = await _db.Colors.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (color == null) return NotFound();
            if (!ModelState.IsValid)
            {
                return View();
            }

            dbColor.HeaderBackgroundColor = color.HeaderBackgroundColor.Trim();
            dbColor.FooterBackgroundColor = color.FooterBackgroundColor.Trim();
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region IsDeleted
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Color color = await _db.Colors
                .FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted == false);

            if (color == null) return NotFound();

            color.IsDeleted = true;
            color.DeletedBy = "system";
            color.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        #endregion
    }
}
