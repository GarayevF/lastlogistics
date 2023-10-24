using ApplianceRepair.Extensions;
using ApplianceRepair.Helpers;
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
	public class IncotermsSectionController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public IncotermsSectionController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        #region Index
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<IncotermsSection> query = _db.IncotermsSections.Where(x => !x.IsDeleted && x.Language.Culture=="az-Latn-AZ");
            return View(PageNatedList<IncotermsSection>.Create(query, pageIndex, 5, 5));
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
        public async Task<IActionResult> Create(List<IncotermsSection> incotermsSection)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }


            #region Photo
            if (incotermsSection[0].Photo != null)
            {
                if (!(incotermsSection[0].Photo.CheckFileContenttype("image/jpeg") || incotermsSection[0].Photo.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo", $"{incotermsSection[0].Photo.FileName} adli fayl novu duzgun deyil");
                    return View(incotermsSection);
                }

                if (incotermsSection[0].Photo.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo", $"{incotermsSection[0].Photo.FileName} adli fayl hecmi coxdur");
                    return View(incotermsSection);
                }

                incotermsSection[0].Icon = await incotermsSection[0].Photo.CreateFileAsync(_env, "assets", "img", "incoterms");
            }
            else
            {
                ModelState.AddModelError("Photo", "Image is empty");
                return View(incotermsSection);
            }
            #endregion

            IncotermsSection test = await _db.IncotermsSections.OrderByDescending(a => a.Id).FirstOrDefaultAsync();
            foreach (IncotermsSection incotermsSections in incotermsSection)
            {
                incotermsSections.Icon = incotermsSection[0].Icon;
                incotermsSections.LanguageGroup = test != null ? test.LanguageGroup + 1 : 1;
                incotermsSections.CreatedAt = DateTime.UtcNow.AddHours(4);
                await _db.IncotermsSections.AddAsync(incotermsSections);
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

            IncotermsSection? firstIncotermsSection = await _db.IncotermsSections.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstIncotermsSection == null) return NotFound();

            List<IncotermsSection> incotermsSection = await _db.IncotermsSections.Where(c => c.LanguageGroup == firstIncotermsSection.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (IncotermsSection incoterm in incotermsSection)
            {
                if (incoterm == null) return NotFound();
            }

            return View(incotermsSection);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, List<IncotermsSection> incotermsSections)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();

            if (!ModelState.IsValid) return View(incotermsSections);

            if (id == null) return BadRequest();

            IncotermsSection? firstIncotermsSection = await _db.IncotermsSections.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstIncotermsSection == null) return NotFound();

            List<IncotermsSection> dbIncotermsSections = new List<IncotermsSection>();

            dbIncotermsSections = await _db.IncotermsSections.Where(c => c.LanguageGroup == firstIncotermsSection.LanguageGroup && c.IsDeleted == false).ToListAsync();

            #region Photo
            if (incotermsSections[0].Photo != null)
            {
                if (!(incotermsSections[0].Photo.CheckFileContenttype("image/jpeg") || incotermsSections[0].Photo.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo", $"{incotermsSections[0].Photo.FileName} adli fayl novu duzgun deyil");
                    return View(incotermsSections);
                }

                if (incotermsSections[0].Photo.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo", $"{incotermsSections[0].Photo.FileName} adli fayl hecmi coxdur");
                    return View(incotermsSections);
                }

                FileHelper.DeleteFile(dbIncotermsSections[0].Icon, _env, "assets", "img", "incoterms");
                foreach (IncotermsSection incotermsSectionImage in dbIncotermsSections)
                {
                    incotermsSectionImage.Icon = await incotermsSections[0].Photo.CreateFileAsync(_env, "assets", "img", "incoterms");
                }
            }
            #endregion

            if (dbIncotermsSections == null || dbIncotermsSections.Count == 0) return NotFound();

            foreach (IncotermsSection incotermsSection in dbIncotermsSections)
            {
                if (incotermsSection == null) return NotFound();
            }

            foreach (IncotermsSection incotermsSection in incotermsSections)
            {
                IncotermsSection? dbIncotermsSection = dbIncotermsSections.FirstOrDefault(s => s.LanguageId == incotermsSection.LanguageId);

                if (dbIncotermsSection == null) return NotFound();
                dbIncotermsSection.Title = incotermsSection.Title.Trim();
                dbIncotermsSection.UpdatedAt = DateTime.UtcNow.AddHours(4);
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

            IncotermsSection? firstIncotermsSection = await _db.IncotermsSections.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstIncotermsSection == null) return NotFound();

            List<IncotermsSection> incotermsSection = await _db.IncotermsSections.Where(c => c.LanguageGroup == firstIncotermsSection.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (IncotermsSection incotermSection in incotermsSection)
            {
                if (incotermSection == null) return NotFound();

                incotermSection.IsDeleted = true;
                incotermSection.DeletedBy = "system";
                incotermSection.DeletedAt = DateTime.UtcNow.AddHours(4);

            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        #endregion

    }
}
