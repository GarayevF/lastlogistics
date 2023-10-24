using System.ComponentModel.DataAnnotations.Schema;

namespace Logistics.Models
{
    public class PriorityCard : BaseEntity
    {
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
        public int LanguageGroup { get; set; }
    }
}
