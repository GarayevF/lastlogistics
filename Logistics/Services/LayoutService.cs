using Logistics.DataAccessLayer;
using Logistics.Interfaces;
using Logistics.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;

namespace Logistics.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context; 

        public LayoutService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetServices()
        {
            IEnumerable<Service> services = await _context.Services.Where(a => !a.IsDeleted && a.Language.Culture == CultureInfo.CurrentCulture.Name).ToListAsync();
            return services;
        }

        public async Task<string> GetCurrentLangauge()
        {
            string CurrentLangauge = CultureInfo.CurrentCulture.Name;
            return CurrentLangauge;
        }

        public async Task<IEnumerable<Setting>> GetSettings()
        {
            IEnumerable<Setting>? settings = await _context.Settings.Include(a => a.Language).ToListAsync();

            return settings;
        }
    }
}
