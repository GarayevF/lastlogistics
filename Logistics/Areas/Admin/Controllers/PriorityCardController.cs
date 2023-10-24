using ApplianceRepair.Extensions;
using ApplianceRepair.Helpers;
using Logistics.DataAccessLayer;
using Logistics.Models;
using Microsoft.AspNetCore.Authorization;
using Logistics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Logistics.Areas.Admin.Controllers
{
    [Area("Admin")]
	//[Authorize(Roles = "SuperAdmin")]
	public class PriorityCardController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public PriorityCardController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;

        }

        #region Index
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<PriorityCard> query = _db.PriorityCards.Where(x => !x.IsDeleted);
            return View(PageNatedList<PriorityCard>.Create(query, pageIndex, 5, 5));
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
        public async Task<IActionResult> Create(List<PriorityCard> priorityCard)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }

            #region Photo
            if (priorityCard[0].Photo != null)
            {
                if (!(priorityCard[0].Photo.CheckFileContenttype("image/jpeg") || priorityCard[0].Photo.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo", $"{priorityCard[0].Photo.FileName} adli fayl novu duzgun deyil");
                    return View(priorityCard);
                }

                if (priorityCard[0].Photo.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo", $"{priorityCard[0].Photo.FileName} adli fayl hecmi coxdur");
                    return View(priorityCard);
                }

                priorityCard[0].Image = await priorityCard[0].Photo.CreateFileAsync(_env, "assets", "img", "prioritycard");
            }
            else
            {
                ModelState.AddModelError("Photo", "Image is empty");
                return View(priorityCard);
            }
            #endregion

            PriorityCard test = await _db.PriorityCards.OrderByDescending(a => a.Id).FirstOrDefaultAsync();
            foreach (PriorityCard priorityCards in priorityCard)
            {
                priorityCards.Image = priorityCard[0].Image;
                priorityCards.LanguageGroup = test != null ? test.LanguageGroup + 1 : 1;
                priorityCards.CreatedAt = DateTime.UtcNow.AddHours(4);
                await _db.PriorityCards.AddAsync(priorityCards);
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

            PriorityCard? firstPriorityCard = await _db.PriorityCards.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstPriorityCard == null) return NotFound();

            List<PriorityCard> priorityCards = await _db.PriorityCards.Where(c => c.LanguageGroup == firstPriorityCard.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (PriorityCard priorityCard in priorityCards)
            {
                if (priorityCard == null) return NotFound();
            }

            return View(priorityCards);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, List<PriorityCard> priorityCards)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();

            if (!ModelState.IsValid) return View(priorityCards);

            if (id == null) return BadRequest();

            PriorityCard? firstPriorityCard = await _db.PriorityCards.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstPriorityCard == null) return NotFound();

            List<PriorityCard> dbPriorityCard = new List<PriorityCard>();

            dbPriorityCard = await _db.PriorityCards.Where(c => c.LanguageGroup == firstPriorityCard.LanguageGroup && c.IsDeleted == false).ToListAsync();

            #region Photo
            if (priorityCards[0].Photo != null)
            {
                if (!(priorityCards[0].Photo.CheckFileContenttype("image/jpeg") || priorityCards[0].Photo.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo", $"{priorityCards[0].Photo.FileName} adli fayl novu duzgun deyil");
                    return View(priorityCards);
                }

                if (priorityCards[0].Photo.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo", $"{priorityCards[0].Photo.FileName} adli fayl hecmi coxdur");
                    return View(priorityCards);
                }
                FileHelper.DeleteFile(dbPriorityCard[0].Image, _env, "assets", "img", "prioritycard");
                foreach (PriorityCard priorityCardImage in dbPriorityCard)
                {
                    priorityCardImage.Image = await priorityCards[0].Photo.CreateFileAsync(_env, "assets", "img", "prioritycard");
                }
            }
            #endregion

            if (dbPriorityCard == null || dbPriorityCard.Count == 0) return NotFound();

            foreach (PriorityCard priorityCard in dbPriorityCard)
            {
                if (priorityCard == null) return NotFound();
            }

            foreach (PriorityCard priorityCard in priorityCards)
            {
                PriorityCard? dbPriorityCards = dbPriorityCard.FirstOrDefault(s => s.LanguageId == priorityCard.LanguageId);

                if (dbPriorityCards == null) return NotFound();
                dbPriorityCards.Title = priorityCard.Title.Trim();
                dbPriorityCards.Description = priorityCard.Description.Trim();
                dbPriorityCards.UpdatedAt = DateTime.UtcNow.AddHours(4);
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

            PriorityCard? firstPriorityCard = await _db.PriorityCards.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstPriorityCard == null) return NotFound();

            List<PriorityCard> priorityCards = await _db.PriorityCards.Where(c => c.LanguageGroup == firstPriorityCard.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (PriorityCard priorityCard in priorityCards)
            {
                if (priorityCard == null) return NotFound();

                priorityCard.IsDeleted = true;
                priorityCard.DeletedBy = "system";
                priorityCard.DeletedAt = DateTime.UtcNow.AddHours(4);

            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        #endregion
    }
}
