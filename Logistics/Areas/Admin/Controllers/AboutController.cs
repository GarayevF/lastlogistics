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
	public class AboutController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public AboutController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        #region Index
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<About> query = _db.Abouts.Where(x => !x.IsDeleted && x.Language.Culture=="az-Latn-AZ");
            return View(PageNatedList<About>.Create(query, pageIndex, 5, 5));
        }
        #endregion

        #region Detail
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            About about = await _db.Abouts.SingleOrDefaultAsync(x => x.Id == id);
            if (about == null)
            {
                return BadRequest();
            }
            return View(about);
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
        public async Task<IActionResult> Create(List<About> about)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }

            #region Photo_1 Create
            if (about[0].Photo_1 != null)
            {
                if (!(about[0].Photo_1.CheckFileContenttype("image/jpeg") || about[0].Photo_1.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo_1", $"{about[0].Photo_1.FileName} adli fayl novu duzgun deyil");
                    return View(about);
                }

                if (about[0].Photo_1.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo_1", $"{about[0].Photo_1.FileName} adli fayl hecmi coxdur");
                    return View(about);
                }

                about[0].Image_1 = await about[0].Photo_1.CreateFileAsync(_env, "assets", "img", "about");
            }
            else
            {
                ModelState.AddModelError("Photo_1", "Image is empty");
                return View(about);
            }
            #endregion

            #region Photo_2 Create
            if (about[0].Photo_2 != null)
            {
                if (!(about[0].Photo_2.CheckFileContenttype("image/jpeg") || about[0].Photo_2.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo_2", $"{about[0].Photo_2.FileName} adli fayl novu duzgun deyil");
                    return View(about);
                }

                if (about[0].Photo_2.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo_2", $"{about[0].Photo_2.FileName} adli fayl hecmi coxdur");
                    return View(about);
                }

                about[0].Image_2 = await about[0].Photo_2.CreateFileAsync(_env, "assets", "img", "about");
            }
            else
            {
                ModelState.AddModelError("Photo_2", "Image is empty");
                return View(about);
            }
            #endregion

            #region Photo_3 Create
            if (about[0].Photo_3 != null)
            {
                if (!(about[0].Photo_3.CheckFileContenttype("image/jpeg") || about[0].Photo_3.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo_3", $"{about[0].Photo_3.FileName} adli fayl novu duzgun deyil");
                    return View(about);
                }

                if (about[0].Photo_3.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo_3", $"{about[0].Photo_3.FileName} adli fayl hecmi coxdur");
                    return View(about);
                }

                about[0].Image_3 = await about[0].Photo_3.CreateFileAsync(_env, "assets", "img", "about");
            }
            else
            {
                ModelState.AddModelError("Photo_3", "Image is empty");
                return View(about);
            }
            #endregion

            About test = await _db.Abouts.OrderByDescending(a => a.Id).FirstOrDefaultAsync();

            foreach (About abouts in about)
            {
                abouts.Image_1 = about[0].Image_1;
                abouts.Image_2 = about[0].Image_2;
                abouts.Image_3 = about[0].Image_3;
                abouts.LanguageGroup = test != null ? test.LanguageGroup + 1 : 1;
                abouts.CreatedAt = DateTime.UtcNow.AddHours(4);
                await _db.Abouts.AddAsync(abouts);
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

            About? firstAbout = await _db.Abouts.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstAbout == null) return NotFound();

            List<About> abouts = await _db.Abouts.Where(c => c.LanguageGroup == firstAbout.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (About about in abouts)
            {
                if (about == null) return NotFound();
            }

            return View(abouts);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, List<About> about)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();

            if (!ModelState.IsValid) return View(about);

            if (id == null) return BadRequest();

            About? firstAbout = await _db.Abouts.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstAbout == null) return NotFound();

            List<About> dbAbouts = new List<About>();

            dbAbouts = await _db.Abouts.Where(c => c.LanguageGroup == firstAbout.LanguageGroup && c.IsDeleted == false).ToListAsync();

            #region Photo_1
            if (about[0].Photo_1 != null)
            {
                if (!(about[0].Photo_1.CheckFileContenttype("image/jpeg") || about[0].Photo_1.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo_1", $"{about[0].Photo_1.FileName} adli fayl novu duzgun deyil");
                    return View(about);
                }

                if (about[0].Photo_1.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo_1", $"{about[0].Photo_1.FileName} adli fayl hecmi coxdur");
                    return View(about);
                }
                FileHelper.DeleteFile(dbAbouts[0].Image_1, _env, "assets", "img", "about");
                foreach (About aboutImage1 in dbAbouts)
                {
                    aboutImage1.Image_1 = await about[0].Photo_1.CreateFileAsync(_env, "assets", "img", "about");
                }
            }
            #endregion

            #region Photo_2
            if (about[0].Photo_2 != null)
            {
                if (!(about[0].Photo_2.CheckFileContenttype("image/jpeg") || about[0].Photo_2.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo_2", $"{about[0].Photo_2.FileName} adli fayl novu duzgun deyil");
                    return View(about);
                }

                if (about[0].Photo_2.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo_2", $"{about[0].Photo_2.FileName} adli fayl hecmi coxdur");
                    return View(about);
                }
                FileHelper.DeleteFile(dbAbouts[0].Image_2, _env, "assets", "img", "about");
                foreach (About aboutImage2 in dbAbouts)
                {
                    aboutImage2.Image_2 = await about[0].Photo_2.CreateFileAsync(_env, "assets", "img", "about");
                }
            }
            #endregion

            #region Photo_3
            if (about[0].Photo_3 != null)
            {
                if (!(about[0].Photo_3.CheckFileContenttype("image/jpeg") || about[0].Photo_3.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo_3", $"{about[0].Photo_3.FileName} adli fayl novu duzgun deyil");
                    return View(about);
                }

                if (about[0].Photo_3.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo_3", $"{about[0].Photo_3.FileName} adli fayl hecmi coxdur");
                    return View(about);
                }
                FileHelper.DeleteFile(dbAbouts[0].Image_3, _env, "assets", "img", "about");
                foreach (About aboutImage3 in dbAbouts)
                {
                    aboutImage3.Image_3 = await about[0].Photo_3.CreateFileAsync(_env, "assets", "img", "about");
                }
            }
            #endregion

            if (dbAbouts == null || dbAbouts.Count == 0) return NotFound();

            foreach (About about1 in dbAbouts)
            {
                if (about1 == null) return NotFound();
            }

            foreach (About about2 in about)
            {
                About? dbAbout = dbAbouts.FirstOrDefault(s => s.LanguageId == about2.LanguageId);

                if (dbAbout == null) return NotFound();
                dbAbout.Title = about2.Title.Trim();
                dbAbout.Description = about2.Description.Trim();
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

            About? firstAbout = await _db.Abouts.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstAbout == null) return NotFound();

            List<About> abouts = await _db.Abouts.Where(c => c.LanguageGroup == firstAbout.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (About about in abouts)
            {
                if (about == null) return NotFound();

                about.IsDeleted = true;
                about.DeletedBy = "system";
                about.DeletedAt = DateTime.UtcNow.AddHours(4);

            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        #endregion
    }
}
