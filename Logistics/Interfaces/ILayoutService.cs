using Logistics.Models;

namespace Logistics.Interfaces
{
    public interface ILayoutService
    {
        Task<IEnumerable<Setting>> GetSettings();
        Task<IEnumerable<Service>> GetServices();
        Task<string> GetCurrentLangauge();

    }
}
