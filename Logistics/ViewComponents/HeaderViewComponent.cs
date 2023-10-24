using Logistics.DataAccessLayer;
using Logistics.Models;
using Logistics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Logistics.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        public HeaderViewComponent(AppDbContext db)
        {
            _db = db;
        }
        private readonly AppDbContext _db;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            string tempName = CultureInfo.CurrentCulture.Name;
            Language tempCulture = await _db?.Languages?.FirstOrDefaultAsync(l => l.Culture == CultureInfo.CurrentCulture.Name);

            HeaderVM headerVM = new HeaderVM
            {
                Color = await _db.Colors.Where(x => !x.IsDeleted).FirstOrDefaultAsync(),
                Services = await _db.Services.Where(x => !x.IsDeleted && x.Language.Culture == CultureInfo.CurrentCulture.Name).ToListAsync(),
                LanguageVMs = new List<LanguageVM>()
            };

            headerVM.CurrentLanguageName = tempCulture.Name;
            headerVM.CurrentLanguageCulture = tempCulture.Culture;

            IEnumerable<Language> languages = await _db.Languages.ToListAsync();

            foreach (Language language in languages)
            {
                if(language.Name != tempName)
                {
                    LanguageVM temp = new LanguageVM()
                    {
                        Name = language.Name,
                        Culture = language.Culture,
                    };
                    headerVM.LanguageVMs.Add(temp);
                }
            }

            return View(headerVM);
        }
    }
}
