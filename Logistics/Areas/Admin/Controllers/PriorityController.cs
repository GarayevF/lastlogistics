using ApplianceRepair.Extensions;
using Logistics.DataAccessLayer;
using Logistics.Models;
using Microsoft.AspNetCore.Authorization;
using Logistics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Areas.Admin.Controllers
{
    [Area("Admin")]
	//[Authorize(Roles = "SuperAdmin")]
	public class PriorityController : Controller
    {
        private readonly AppDbContext _db;

        public PriorityController(AppDbContext db)
        {
            _db = db;
        }

        #region Index
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Priority> query = _db.Priorities.Where(x => !x.IsDeleted && x.Language.Culture=="az-Latn-AZ");
            return View(PageNatedList<Priority>.Create(query, pageIndex, 5, 5));
        }
        #endregion

        #region Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<Priority> priority)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }
            Priority test = await _db.Priorities.OrderByDescending(a => a.Id).FirstOrDefaultAsync();
            foreach (Priority priorities in priority)
            {
                priorities.LanguageGroup = test != null ? test.LanguageGroup + 1 : 1;
                priorities.CreatedAt = DateTime.UtcNow.AddHours(4);
                await _db.Priorities.AddAsync(priorities);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();

            if (id == null) return BadRequest();

            Priority? firstPriority = await _db.Priorities.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstPriority == null) return NotFound();

            List<Priority> priorities = await _db.Priorities.Where(c => c.LanguageGroup == firstPriority.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (Priority priority in priorities)
            {
                if (priority == null) return NotFound();
            }

            return View(priorities);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, List<Priority> priorities)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();

            if (!ModelState.IsValid) return View(priorities);

            if (id == null) return BadRequest();

            Priority? firstPriority = await _db.Priorities.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstPriority == null) return NotFound();

            List<Priority> dbPriorities = new List<Priority>();

            dbPriorities = await _db.Priorities.Where(c => c.LanguageGroup == firstPriority.LanguageGroup && c.IsDeleted == false).ToListAsync();

            if (dbPriorities == null || dbPriorities.Count == 0) return NotFound();

            foreach (Priority priority in dbPriorities)
            {
                if (priority == null) return NotFound();
            }

            foreach (Priority priority in priorities)
            {
                Priority? dbPriority = dbPriorities.FirstOrDefault(s => s.LanguageId == priority.LanguageId);

                if (dbPriority == null) return NotFound();
                dbPriority.Description = priority.Description.Trim();
                dbPriority.UpdatedAt = DateTime.UtcNow.AddHours(4);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Deleted
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            List<Language> languages = await _db.Languages.ToListAsync();
            ViewBag.Languages = languages;

            Priority? firstPriority = await _db.Priorities.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstPriority == null) return NotFound();

            List<Priority> priorities = await _db.Priorities.Where(c => c.LanguageGroup == firstPriority.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (Priority priority in priorities)
            {
                if (priority == null) return NotFound();

                priority.IsDeleted = true;
                priority.DeletedBy = "system";
                priority.DeletedAt = DateTime.UtcNow.AddHours(4);

            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        #endregion
    }
}
