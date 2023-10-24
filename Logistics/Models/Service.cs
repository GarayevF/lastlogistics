using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logistics.Models
{
    public class Service : BaseEntity
    {
        [StringLength(100)]
        public string Title { get; set; }
        [StringLength(1000)]
        public string? Icon { get; set; }
        [NotMapped]
        public IFormFile? IconPhoto { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        public string Content { get; set; }
        public string? Image_1 { get; set; }
        [NotMapped]
        public IFormFile? Photo_1 { get; set; }
        public string? Image_2 { get; set; }
        [NotMapped]
        public IFormFile? Photo_2 { get; set; }
        public string? Image_3 { get; set; }
        [NotMapped]
        public IFormFile? Photo_3 { get; set; }
        public string SectionDescription { get; set; }
        public bool IsBaseDesign { get; set; } = true;
        [NotMapped]
        public IEnumerable<int> ServiceSectionIds { get; set; }
        public List<ServiceServiceSection>? ServiceServiceSections { get; set; }
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
        public int? LanguageGroup { get; set; }
    }
}
