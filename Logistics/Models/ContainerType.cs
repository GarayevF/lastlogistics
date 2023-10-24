using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logistics.Models
{
    public class ContainerType : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Icon { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
        public int LanguageGroup { get; set; }
    }
}
