using Logistics.DataAccessLayer;
using Logistics.Models;
using Logistics.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Areas.Admin.Controllers
{
    [Area("Admin")]
	//[Authorize(Roles = "SuperAdmin")]
	public class ServiceSectionsController : Controller
    {
        private readonly AppDbContext _db;
        public ServiceSectionsController(AppDbContext db)
        {
            _db = db;
        }

        #region Index
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<ServiceSection> query = _db.ServiceSections.Where(x => !x.IsDeleted);
            return View(PageNatedList<ServiceSection>.Create(query, pageIndex, 5, 5));
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
        public async Task<IActionResult> Create(List<ServiceSection> serviceSections)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }

            ServiceSection? test = await _db.ServiceSections.OrderByDescending(a => a.Id).FirstOrDefaultAsync();

            foreach (ServiceSection serviceSection in serviceSections)
            {
                serviceSection.LanguageGroup = test != null ? test.LanguageGroup + 1 : 1;
                serviceSection.CreatedAt = DateTime.UtcNow.AddHours(4);
                await _db.ServiceSections.AddAsync(serviceSection);
            }

            
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(int? id)
        {

            ViewBag.Languages = await _db.Languages.ToListAsync();

            if (id == null) return BadRequest();

            ServiceSection? firstServiceSection = await _db.ServiceSections.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstServiceSection == null) return NotFound();

            List<ServiceSection> serviceSections = await _db.ServiceSections.Where(c => c.LanguageGroup == firstServiceSection.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (ServiceSection service in serviceSections)
            {
                if (service == null) return NotFound();
            }

            return View(serviceSections);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, List<ServiceSection> serviceSections)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();

            if (!ModelState.IsValid) return View(serviceSections);

            if (id == null) return BadRequest();

            ServiceSection? firstServiceSection = await _db.ServiceSections.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstServiceSection == null) return NotFound();

            List<ServiceSection> dbServiceSections = new List<ServiceSection>();

            dbServiceSections = await _db.ServiceSections.Where(c => c.LanguageGroup == firstServiceSection.LanguageGroup && c.IsDeleted == false).ToListAsync();

            if (dbServiceSections == null || dbServiceSections.Count == 0) return NotFound();

            foreach (ServiceSection serviceSection in dbServiceSections)
            {
                if (serviceSection == null) return NotFound();
            }

            foreach (ServiceSection serviceSection in serviceSections)
            {
                ServiceSection dbService = dbServiceSections.FirstOrDefault(s => s.LanguageId == serviceSection.LanguageId);

                dbService.Title = serviceSection.Title.Trim();
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region IsDeleted
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null) return BadRequest();

            List<Language> languages = await _db.Languages.ToListAsync();
            ViewBag.Languages = languages;

            ServiceSection? firstServiceSection = await _db.ServiceSections.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstServiceSection == null) return NotFound();

            List<ServiceSection> serviceSections = await _db.ServiceSections.Where(c => c.LanguageGroup == firstServiceSection.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (ServiceSection serviceSection1 in serviceSections)
            {
                if (serviceSection1 == null) return NotFound();

                serviceSection1.IsDeleted = true;
                serviceSection1.DeletedBy = "system";
                serviceSection1.DeletedAt = DateTime.UtcNow.AddHours(4);
                
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        #endregion
    }
}