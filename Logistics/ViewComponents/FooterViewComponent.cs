using Logistics.DataAccessLayer;
using Logistics.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Logistics.ViewComponents
{
    public class FooterViewComponent:ViewComponent
    {
        public FooterViewComponent(AppDbContext db)
        {
            _db = db;
        }
        private readonly AppDbContext _db;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Color color = await _db.Colors.Where(x=>!x.IsDeleted).FirstOrDefaultAsync();
            return View(color);
        }
    }
}
