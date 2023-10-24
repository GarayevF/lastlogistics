using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logistics.Models
{
    public class IncotermsSection : BaseEntity
    {
        public string? Icon { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }
        public string Title { get; set; }
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
        public int LanguageGroup { get; set; }
    }
}
