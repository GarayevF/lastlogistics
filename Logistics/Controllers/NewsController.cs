using Logistics.DataAccessLayer;
using Logistics.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Logistics.Controllers
{
    public class NewsController : Controller
    {
        private readonly AppDbContext _db;
        public NewsController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            List<NewsContent> newsContents = await _db.NewsContents.Where(x => !x.IsDeleted && x.Language.Culture == CultureInfo.CurrentCulture.Name).OrderByDescending(x=>x.CreatedAt).ToListAsync();
            return View(newsContents);
        }
    }
}
