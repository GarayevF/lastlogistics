using Logistics.Models;
using System.Drawing;

namespace Logistics.ViewModels
{
    public class HeaderVM
    {
        public Models.Color Color { get; set; }
        public List<Service>? Services { get; set; }
        public List<LanguageVM>? LanguageVMs { get; set; }
        public string CurrentLanguageName { get; set; }
        public string CurrentLanguageCulture { get; set; }
    }
}
