using System.ComponentModel.DataAnnotations.Schema;

namespace Logistics.Models
{
    public class About : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Image_1 { get; set; }
        public string? Image_2 { get; set; }
        public string? Image_3 { get; set; }
        [NotMapped]
        public IFormFile? Photo_1 { get; set; }
        [NotMapped]
        public IFormFile? Photo_2 { get; set; }
        [NotMapped]
        public IFormFile? Photo_3 { get; set; }
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
        public int LanguageGroup { get; set; }
    }
}
