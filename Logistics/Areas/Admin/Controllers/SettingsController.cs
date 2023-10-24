using Logistics.DataAccessLayer;
using Logistics.Models;
using Logistics.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Logistics.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public SettingsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        #region Index
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Setting> query = _db.Settings
                .OrderByDescending(c => c.Id);

            return View(PageNatedList<Setting>.Create(query, pageIndex, 5, 5));
        }
        #endregion

        #region Create
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create(List<Setting> settings)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();
            if (!ModelState.IsValid) return View(settings);

            if (await _db.Settings.AnyAsync(c => c.Key.ToLower() == settings[0].Key.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"Bu {settings[0].Key} key movcuddur");
                return View(settings);
            }

            Setting test = await _db.Settings.OrderByDescending(a => a.Id).FirstOrDefaultAsync();
            foreach (Setting setting in settings)
            {
                setting.Key = settings[0].Key.Trim();
                setting.LanguageGroup = test != null ? test.LanguageGroup + 1 : 1;
                await _db.Settings.AddAsync(setting);
            }


            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Update
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int? id)
        {

            ViewBag.Languages = await _db.Languages.ToListAsync();

            if (id == null) return BadRequest();

            Setting? firstSettings = await _db.Settings.FirstOrDefaultAsync(c => c.Id == id);

            if (firstSettings == null) return NotFound();

            List<Setting> settings = await _db.Settings.Where(c => c.LanguageGroup == firstSettings.LanguageGroup).ToListAsync();

            foreach (Setting setting in settings)
            {
                if (setting == null) return NotFound();
            }

            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int? id, List<Setting> settings)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();

            if (!ModelState.IsValid) return View(settings);

            if (id == null) return BadRequest();

            Setting? firstSetting = await _db.Settings.FirstOrDefaultAsync(c => c.Id == id);

            if (firstSetting == null) return NotFound();

            List<Setting> dbSettings = new List<Setting>();

            dbSettings = await _db.Settings.Where(c => c.LanguageGroup == firstSetting.LanguageGroup).ToListAsync();

            if (dbSettings == null || dbSettings.Count == 0) return NotFound();

            foreach (Setting setting in dbSettings)
            {
                if (setting == null) return NotFound();
            }

            foreach (Setting setting in settings)
            {
                Setting? dbSetting = dbSettings.FirstOrDefault(s => s.LanguageId == setting.LanguageId);

                if (dbSetting == null) return NotFound();
                dbSetting.Key = setting.Key.Trim();
                dbSetting.Value = setting.Value;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Setting setting = await _db.Settings
                .FirstOrDefaultAsync(c => c.Id == id);

            if (setting == null) return NotFound();

            _db.Settings.Remove(setting);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
