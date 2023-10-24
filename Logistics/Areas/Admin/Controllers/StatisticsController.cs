using ApplianceRepair.Extensions;
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
	public class StatisticsController : Controller
    {
        private readonly AppDbContext _db;
        public StatisticsController(AppDbContext db)
        {
            _db = db;
        }

        #region Index
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Statistics> query = _db.Statistics.Where(x => !x.IsDeleted && x.Language.Culture=="az-Latn-AZ");
            return View(PageNatedList<Statistics>.Create(query, pageIndex, 5, 5));
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
        public async Task<IActionResult> Create(List<Statistics> statistics)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }

            Statistics test = await _db.Statistics.OrderByDescending(a => a.Id).FirstOrDefaultAsync();

            foreach (Statistics statistic in statistics)
            {
                statistic.Value = statistics[0].Value;
                statistic.LanguageGroup = test != null ? test.LanguageGroup + 1 : 1;
                statistic.CreatedAt = DateTime.UtcNow.AddHours(4);
                await _db.Statistics.AddAsync(statistic);
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

            Statistics? firstStatistics = await _db.Statistics.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstStatistics == null) return NotFound();

            List<Statistics> statistics = await _db.Statistics.Where(c => c.LanguageGroup == firstStatistics.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (Statistics statisticsContent in statistics)
            {
                if (statisticsContent == null) return NotFound();
            }

            return View(statistics);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, List<Statistics> statistics)
        {
            ViewBag.Languages = await _db.Languages.ToListAsync();

            if (!ModelState.IsValid) return View(statistics);

            if (id == null) return BadRequest();

            Statistics? firstStatistics = await _db.Statistics.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstStatistics == null) return NotFound();

            List<Statistics> dbStatistics = new List<Statistics>();

            dbStatistics = await _db.Statistics.Where(c => c.LanguageGroup == firstStatistics.LanguageGroup && c.IsDeleted == false).ToListAsync();

            if (dbStatistics == null || dbStatistics.Count == 0) return NotFound();

            foreach (Statistics statisticsContent in dbStatistics)
            {
                if (statisticsContent == null) return NotFound();
            }

            foreach (Statistics statisticsContent in statistics)
            {
                Statistics? dbStatisticsContent = dbStatistics.FirstOrDefault(s => s.LanguageId == statisticsContent.LanguageId);

                if (dbStatisticsContent == null) return NotFound();
                dbStatisticsContent.Title = statisticsContent.Title.Trim();
                dbStatisticsContent.Value = statisticsContent.Value;
                dbStatisticsContent.UpdatedAt = DateTime.UtcNow.AddHours(4);
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

            Statistics? firstStatistics = await _db.Statistics.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstStatistics == null) return NotFound();

            List<Statistics> statistics = await _db.Statistics.Where(c => c.LanguageGroup == firstStatistics.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (Statistics statisticsContent in statistics)
            {
                if (statisticsContent == null) return NotFound();

                statisticsContent.IsDeleted = true;
                statisticsContent.DeletedBy = "system";
                statisticsContent.DeletedAt = DateTime.UtcNow.AddHours(4);

            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        #endregion
    }
}
