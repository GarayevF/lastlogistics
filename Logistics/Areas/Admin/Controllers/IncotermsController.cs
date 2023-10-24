using ApplianceRepair.Extensions;
using ApplianceRepair.Helpers;
using Logistics.DataAccessLayer;
using Logistics.Models;
using Microsoft.AspNetCore.Authorization;
using Logistics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using static NuGet.Packaging.PackagingConstants;
using System.Threading.Tasks;

namespace Logistics.Areas.Admin.Controllers
{
    [Area("Admin")]
	//[Authorize(Roles = "SuperAdmin")]
	public class IncotermsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public IncotermsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;

        }
        #region Index
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Incoterms> query = _db.Incoterms.Where(x => !x.IsDeleted && x.Language.Culture=="az-Latn-AZ");
            return View(PageNatedList<Incoterms>.Create(query, pageIndex, 5, 5));
        }
        #endregion

        #region Detail
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Incoterms incoterms = await _db.Incoterms.SingleOrDefaultAsync(x => x.Id == id);
            if (incoterms == null)
            {
                return BadRequest();
            }
            return View(incoterms);
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
        public async Task<IActionResult> Create(List<Incoterms> incoterms)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }

            #region AboutPhoto_1 Create
            if (incoterms[0].AboutPhoto_1 != null)
            {
                if (!(incoterms[0].AboutPhoto_1.CheckFileContenttype("image/jpeg") || incoterms[0].AboutPhoto_1.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("AboutPhoto_1", $"{incoterms[0].AboutPhoto_1.FileName} adli fayl novu duzgun deyil");
                    return View(incoterms);
                }

                if (incoterms[0].AboutPhoto_1.CheckFileLength(10240))
                {
                    ModelState.AddModelError("AboutPhoto_1", $"{incoterms[0].AboutPhoto_1.FileName} adli fayl hecmi coxdur");
                    return View(incoterms);
                }

                incoterms[0].AboutImage_1 = await incoterms[0].AboutPhoto_1.CreateFileAsync(_env, "assets", "img", "incoterms");
            }
            else
            {
                ModelState.AddModelError("AboutPhoto_1", "Image is empty");
                return View(incoterms);
            }
            #endregion

            #region AboutPhoto_2 Create
            if (incoterms[0].AboutPhoto_2 != null)
            {
                if (!(incoterms[0].AboutPhoto_2.CheckFileContenttype("image/jpeg") || incoterms[0].AboutPhoto_2.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("AboutPhoto_2", $"{incoterms[0].AboutPhoto_2.FileName} adli fayl novu duzgun deyil");
                    return View(incoterms);
                }

                if (incoterms[0].AboutPhoto_2.CheckFileLength(10240))
                {
                    ModelState.AddModelError("AboutPhoto_2", $"{incoterms[0].AboutPhoto_2.FileName} adli fayl hecmi coxdur");
                    return View(incoterms);
                }

                incoterms[0].AboutImage_2 = await incoterms[0].AboutPhoto_2.CreateFileAsync(_env, "assets", "img", "incoterms");
            }
            else
            {
                ModelState.AddModelError("AboutPhoto_2", "Image is empty");
                return View(incoterms);
            }
            #endregion

            #region AboutPhoto_3 Create
            if (incoterms[0].AboutPhoto_3 != null)
            {
                if (!(incoterms[0].AboutPhoto_3.CheckFileContenttype("image/jpeg") || incoterms[0].AboutPhoto_3.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("AboutPhoto_3", $"{incoterms[0].AboutPhoto_3.FileName} adli fayl novu duzgun deyil");
                    return View(incoterms);
                }

                if (incoterms[0].AboutPhoto_2.CheckFileLength(10240))
                {
                    ModelState.AddModelError("AboutPhoto_3", $"{incoterms[0].AboutPhoto_3.FileName} adli fayl hecmi coxdur");
                    return View(incoterms);
                }

                incoterms[0].AboutImage_3 = await incoterms[0].AboutPhoto_3.CreateFileAsync(_env, "assets", "img", "incoterms");
            }
            else
            {
                ModelState.AddModelError("AboutPhoto_3", "Image is empty");
                return View(incoterms);
            }
            #endregion

            #region ConditionPhoto Create
            if (incoterms[0].ConditionPhoto != null)
            {
                if (!(incoterms[0].ConditionPhoto.CheckFileContenttype("image/jpeg") || incoterms[0].ConditionPhoto.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("ConditionPhoto", $"{incoterms[0].ConditionPhoto.FileName} adli fayl novu duzgun deyil");
                    return View(incoterms);
                }

                if (incoterms[0].AboutPhoto_2.CheckFileLength(10240))
                {
                    ModelState.AddModelError("ConditionPhoto", $"{incoterms[0].ConditionPhoto.FileName} adli fayl hecmi coxdur");
                    return View(incoterms);
                }

                incoterms[0].ConditionImage = await incoterms[0].ConditionPhoto.CreateFileAsync(_env, "assets", "img", "incoterms");
            }
            else
            {
                ModelState.AddModelError("ConditionPhoto", "Image is empty");
                return View(incoterms);
            }
            #endregion

            Incoterms test = await _db.Incoterms.OrderByDescending(a => a.Id).FirstOrDefaultAsync();
            foreach (Incoterms incoterm in incoterms)
            {
                incoterm.AboutImage_1 = incoterms[0].AboutImage_1;
                incoterm.AboutImage_2 = incoterms[0].AboutImage_2;
                incoterm.AboutImage_3 = incoterms[0].AboutImage_3;
                incoterm.LanguageGroup = test != null ? test.LanguageGroup + 1 : 1;
                incoterm.ConditionImage = incoterms[0].ConditionImage;
                incoterm.CreatedAt = DateTime.UtcNow.AddHours(4);
                await _db.Incoterms.AddAsync(incoterm);
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

            Incoterms? firstIncoterms = await _db.Incoterms.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstIncoterms == null) return NotFound();

            List<Incoterms> incoterms = await _db.Incoterms.Where(c => c.LanguageGroup == firstIncoterms.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (Incoterms incoterm in incoterms)
            {
                if (incoterm == null) return NotFound();
            }

            return View(incoterms);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, List<Incoterms> incoterms)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();

            if (!ModelState.IsValid) return View(incoterms);

            if (id == null) return BadRequest();

            Incoterms? firstIncoterms = await _db.Incoterms.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstIncoterms == null) return NotFound();

            List<Incoterms> dbIncoterms = new List<Incoterms>();

            dbIncoterms = await _db.Incoterms.Where(c => c.LanguageGroup == firstIncoterms.LanguageGroup && c.IsDeleted == false).ToListAsync();

            #region AboutPhoto_1
            if (incoterms[0].AboutPhoto_1 != null)
            {
                if (!(incoterms[0].AboutPhoto_1.CheckFileContenttype("image/jpeg") || incoterms[0].AboutPhoto_1.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("AboutPhoto_1", $"{incoterms[0].AboutPhoto_1.FileName} adli fayl novu duzgun deyil");
                    return View(incoterms);
                }

                if (incoterms[0].AboutPhoto_1.CheckFileLength(10240))
                {
                    ModelState.AddModelError("AboutPhoto_1", $"{incoterms[0].AboutPhoto_1.FileName} adli fayl hecmi coxdur");
                    return View(incoterms);
                }

                FileHelper.DeleteFile(dbIncoterms[0].AboutImage_1, _env, "assets", "img", "incoterms");
                foreach (Incoterms incotermsImage in dbIncoterms)
                {
                    incotermsImage.AboutImage_1 = await incoterms[0].AboutPhoto_1.CreateFileAsync(_env, "assets", "img", "incoterms");
                }
            }
            #endregion

            #region AboutPhoto_2
            if (incoterms[0].AboutPhoto_2 != null)
            {
                if (!(incoterms[0].AboutPhoto_2.CheckFileContenttype("image/jpeg") || incoterms[0].AboutPhoto_2.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("AboutPhoto_2", $"{incoterms[0].AboutPhoto_2.FileName} adli fayl novu duzgun deyil");
                    return View(incoterms);
                }

                if (incoterms[0].AboutPhoto_2.CheckFileLength(10240))
                {
                    ModelState.AddModelError("AboutPhoto_2", $"{incoterms[0].AboutPhoto_2.FileName} adli fayl hecmi coxdur");
                    return View(incoterms);
                }
                FileHelper.DeleteFile(dbIncoterms[0].AboutImage_2, _env, "assets", "img", "incoterms");
                foreach (Incoterms incotermsImage in dbIncoterms)
                {
                    incotermsImage.AboutImage_2 = await incoterms[0].AboutPhoto_2.CreateFileAsync(_env, "assets", "img", "incoterms");
                }
            }
            #endregion

            #region AboutPhoto_3
            if (incoterms[0].AboutPhoto_3 != null)
            {
                if (!(incoterms[0].AboutPhoto_3.CheckFileContenttype("image/jpeg") || incoterms[0].AboutPhoto_3.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("AboutPhoto_3", $"{incoterms[0].AboutPhoto_2.FileName} adli fayl novu duzgun deyil");
                    return View(incoterms);
                }
                if (incoterms[0].AboutPhoto_3.CheckFileLength(10240))
                {
                    ModelState.AddModelError("AboutPhoto_3", $"{incoterms[0].AboutPhoto_3.FileName} adli fayl hecmi coxdur");
                    return View(incoterms);
                }
                FileHelper.DeleteFile(dbIncoterms[0].AboutImage_3, _env, "assets", "img", "incoterms");
                foreach (Incoterms incotermsImage in dbIncoterms)
                {
                    incotermsImage.AboutImage_3 = await incoterms[0].AboutPhoto_3.CreateFileAsync(_env, "assets", "img", "incoterms");
                }
            }
            #endregion

            #region ConditionPhoto
            if (incoterms[0].ConditionPhoto != null)
            {
                if (!(incoterms[0].ConditionPhoto.CheckFileContenttype("image/jpeg") || incoterms[0].ConditionPhoto.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("ConditionPhoto", $"{incoterms[0].ConditionPhoto.FileName} adli fayl novu duzgun deyil");
                    return View(incoterms);
                }

                if (incoterms[0].ConditionPhoto.CheckFileLength(10240))
                {
                    ModelState.AddModelError("ConditionPhoto", $"{incoterms[0].ConditionPhoto.FileName} adli fayl hecmi coxdur");
                    return View(incoterms);
                }

                FileHelper.DeleteFile(dbIncoterms[0].ConditionImage, _env, "assets", "img", "incoterms");
                foreach (Incoterms incotermsImage in dbIncoterms)
                {
                    incotermsImage.ConditionImage = await incoterms[0].ConditionPhoto.CreateFileAsync(_env, "assets", "img", "incoterms");
                }
            }
            #endregion

            if (dbIncoterms == null || dbIncoterms.Count == 0) return NotFound();

            foreach (Incoterms incoterm in dbIncoterms)
            {
                if (incoterm == null) return NotFound();
            }

            foreach (Incoterms incoterm in incoterms)
            {
                Incoterms? dbIncoterm = dbIncoterms.FirstOrDefault(s => s.LanguageId == incoterm.LanguageId);

                if (dbIncoterm == null) return NotFound();
                dbIncoterm.AboutDescription = incoterm.AboutDescription.Trim();
                dbIncoterm.ConditionDescription = incoterm.ConditionDescription.Trim();
                dbIncoterm.UpdatedAt = DateTime.UtcNow.AddHours(4);
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

            Incoterms? firstIncoterms = await _db.Incoterms.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstIncoterms == null) return NotFound();

            List<Incoterms> incoterms = await _db.Incoterms.Where(c => c.LanguageGroup == firstIncoterms.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (Incoterms incoterm in incoterms)
            {
                if (incoterm == null) return NotFound();

                incoterm.IsDeleted = true;
                incoterm.DeletedBy = "system";
                incoterm.DeletedAt = DateTime.UtcNow.AddHours(4);

            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
