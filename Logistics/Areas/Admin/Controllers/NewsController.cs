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
	public class NewsController : Controller
    {
        public NewsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        #region Index
        public async Task<IActionResult> Index(int pageIndex = 1)
        {

            IQueryable<NewsContent> query = _db.NewsContents.Where(x => !x.IsDeleted && x.Language.Culture=="az-Latn-AZ");
            return View(PageNatedList<NewsContent>.Create(query, pageIndex, 5, 5));
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
        public async Task<IActionResult> Create(List<NewsContent> news)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }

            #region Photo
            if (news[0].Photo != null)
            {
                if (!(news[0].Photo.CheckFileContenttype("image/jpeg") || news[0].Photo.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo", $"{news[0].Photo.FileName} adli fayl novu duzgun deyil");
                    return View(news);
                }

                if (news[0].Photo.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo", $"{news[0].Photo.FileName} adli fayl hecmi coxdur");
                    return View(news);
                }

                news[0].Image = await news[0].Photo.CreateFileAsync(_env, "assets", "img", "news");
            }
            else
            {
                ModelState.AddModelError("Photo", "Image is empty");
                return View(news);
            }
            #endregion

            NewsContent test = await _db.NewsContents.OrderByDescending(a => a.Id).FirstOrDefaultAsync();
            foreach (NewsContent newsContent in news)
            {
                newsContent.Image = news[0].Image;
                newsContent.Date = news[0].Date;
                newsContent.LanguageGroup = test != null ? test.LanguageGroup + 1 : 1;
                newsContent.CreatedAt = DateTime.UtcNow.AddHours(4);
                await _db.NewsContents.AddAsync(newsContent);
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

            NewsContent? firstNews = await _db.NewsContents.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstNews == null) return NotFound();

            List<NewsContent> news = await _db.NewsContents.Where(c => c.LanguageGroup == firstNews.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (NewsContent news1 in news)
            {
                if (news1 == null) return NotFound();
            }

            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, List<NewsContent> news)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();

            if (!ModelState.IsValid) return View(news);

            if (id == null) return BadRequest();

            NewsContent? firstNews = await _db.NewsContents.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstNews == null) return NotFound();

            List<NewsContent> dbNews = new List<NewsContent>();

            dbNews = await _db.NewsContents.Where(c => c.LanguageGroup == firstNews.LanguageGroup && c.IsDeleted == false).ToListAsync();

            #region Photo
            if (news[0].Photo != null)
            {
                if (!(news[0].Photo.CheckFileContenttype("image/jpeg") || news[0].Photo.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo", $"{news[0].Photo.FileName} adli fayl novu duzgun deyil");
                    return View(news);
                }

                if (news[0].Photo.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo", $"{news[0].Photo.FileName} adli fayl hecmi coxdur");
                    return View(news);
                }
                FileHelper.DeleteFile(dbNews[0].Image, _env, "assets", "img", "news");
                foreach (NewsContent newsImage in dbNews)
                {
                    newsImage.Image = await news[0].Photo.CreateFileAsync(_env, "assets", "img", "news");
                }
            }
            #endregion

            if (dbNews == null || dbNews.Count == 0) return NotFound();

            foreach (NewsContent news1 in dbNews)
            {
                if (news1 == null) return NotFound();
            }

            foreach (NewsContent newsContent in news)
            {
                NewsContent? dbNew = dbNews.FirstOrDefault(s => s.LanguageId == newsContent.LanguageId);

                if (dbNew == null) return NotFound();
                dbNew.Title = newsContent.Title.Trim();
                dbNew.Description = newsContent.Description.Trim();
                dbNew.Date = newsContent.Date;
                dbNew.UpdatedAt = DateTime.UtcNow.AddHours(4);
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

            NewsContent? firstNews = await _db.NewsContents.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstNews == null) return NotFound();

            List<NewsContent> news = await _db.NewsContents.Where(c => c.LanguageGroup == firstNews.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (NewsContent newsContent in news)
            {
                if (newsContent == null) return NotFound();

                newsContent.IsDeleted = true;
                newsContent.DeletedBy = "system";
                newsContent.DeletedAt = DateTime.UtcNow.AddHours(4);

            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        #endregion

    }
}
