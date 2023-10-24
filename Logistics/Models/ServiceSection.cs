using System.ComponentModel.DataAnnotations.Schema;

namespace Logistics.Models
{
    public class ServiceSection : BaseEntity
    {
        public string Title { get; set; }
        [NotMapped]
        public IEnumerable<int>? ServiceIds { get; set; }

        public IEnumerable<ServiceServiceSection>? ServiceServiceSections { get; set; }
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
        public int LanguageGroup { get; set; }

    }
}
