using Logistics.Models;

namespace Logistics.Interfaces
{
    public interface ILocalizationService
    {
        StringResource GetStringResource(string resourceKey, int languageId);
    }
}
