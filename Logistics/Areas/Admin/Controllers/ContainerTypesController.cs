using ApplianceRepair.Extensions;
using ApplianceRepair.Helpers;
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
    //[Authorize(Roles = "SuperAdmin")]
    public class ContainerTypesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public ContainerTypesController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        #region Index
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<ContainerType> query = _db.ContainerTypes.Where(x => !x.IsDeleted && x.Language.Culture=="az-Latn-AZ");
            return View(PageNatedList<ContainerType>.Create(query, pageIndex, 5, 5));
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
        public async Task<IActionResult> Create(List<ContainerType> containerType)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(containerType);
            }

            #region Icon
            if (containerType[0].Photo != null)
            {
                if (!(containerType[0].Photo.CheckFileContenttype("image/jpeg") || containerType[0].Photo.CheckFileContenttype("image/png") || containerType[0].Photo.CheckFileContenttype("image/svg")))
                {
                    ModelState.AddModelError("Photo", $"{containerType[0].Photo.FileName} adli fayl novu duzgun deyil");
                    return View(containerType);
                }

                if (containerType[0].Photo.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo", $"{containerType[0].Photo.FileName} adli fayl hecmi coxdur");
                    return View(containerType);
                }

                containerType[0].Icon = await containerType[0].Photo.CreateFileAsync(_env, "assets", "img", "containerTypes");
            }
            else
            {
                ModelState.AddModelError("Photo", "Image is empty");
                return View(containerType);
            }
            #endregion

            ContainerType test = await _db.ContainerTypes.OrderByDescending(a => a.Id).FirstOrDefaultAsync();
            foreach (ContainerType containerTypes in containerType)
            {
                containerTypes.Icon = containerType[0].Icon;
                containerTypes.LanguageGroup = test != null ? test.LanguageGroup + 1 : 1;
                containerTypes.CreatedAt = DateTime.UtcNow.AddHours(4);
                await _db.ContainerTypes.AddAsync(containerTypes);
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

            ContainerType? firstContainerType = await _db.ContainerTypes.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstContainerType == null) return NotFound();

            List<ContainerType> containerTypes = await _db.ContainerTypes.Where(c => c.LanguageGroup == firstContainerType.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (ContainerType containerType in containerTypes)
            {
                if (containerType == null) return NotFound();
            }

            return View(containerTypes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, List<ContainerType> containerTypes)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();

            if (!ModelState.IsValid) return View(containerTypes);

            if (id == null) return BadRequest();

            ContainerType? firstContainerType = await _db.ContainerTypes.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstContainerType == null) return NotFound();

            List<ContainerType> dbContainerTypes = new List<ContainerType>();

            dbContainerTypes = await _db.ContainerTypes.Where(c => c.LanguageGroup == firstContainerType.LanguageGroup && c.IsDeleted == false).ToListAsync();

            #region Icon
            if (containerTypes[0].Photo != null)
            {
                if (!(containerTypes[0].Photo.CheckFileContenttype("image/jpeg") || containerTypes[0].Photo.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo", $"{containerTypes[0].Photo.FileName} adli fayl novu duzgun deyil");
                    return View(containerTypes);
                }

                if (containerTypes[0].Photo.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo", $"{containerTypes[0].Photo.FileName} adli fayl hecmi coxdur");
                    return View(containerTypes);
                }
                FileHelper.DeleteFile(dbContainerTypes[0].Icon, _env, "assets", "img", "about");
                foreach (ContainerType containerType in dbContainerTypes)
                {
                    containerType.Icon = await containerTypes[0].Photo.CreateFileAsync(_env, "assets", "img", "containerTypes");
                }
            }
            #endregion

            if (dbContainerTypes == null || dbContainerTypes.Count == 0) return NotFound();

            foreach (ContainerType containerType in dbContainerTypes)
            {
                if (containerType == null) return NotFound();
            }

            foreach (ContainerType containerType in containerTypes)
            {
                ContainerType? dbContainerType = dbContainerTypes.FirstOrDefault(s => s.LanguageId == containerType.LanguageId);

                if (dbContainerType == null) return NotFound();
                dbContainerType.Title = containerType.Title.Trim();
                dbContainerType.Description = containerType.Description.Trim();
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

            ContainerType? firstContainerType = await _db.ContainerTypes.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstContainerType == null) return NotFound();

            List<ContainerType> containerTypes = await _db.ContainerTypes.Where(c => c.LanguageGroup == firstContainerType.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (ContainerType containerType in containerTypes)
            {
                if (containerType == null) return NotFound();

                containerType.IsDeleted = true;
                containerType.DeletedBy = "system";
                containerType.DeletedAt = DateTime.UtcNow.AddHours(4);

            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        #endregion
    }
}
