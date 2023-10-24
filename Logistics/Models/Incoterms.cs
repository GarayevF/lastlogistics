using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logistics.Models
{
    public class Incoterms : BaseEntity
    {
        public string AboutDescription { get; set; }
        public string? AboutImage_1 { get; set; }
        [NotMapped]
        public IFormFile? AboutPhoto_1 { get; set; }
        public string? AboutImage_2 { get; set; }
        [NotMapped]
        public IFormFile? AboutPhoto_2 { get; set; }
        public string? AboutImage_3 { get; set; }
        [NotMapped]
        public IFormFile? AboutPhoto_3 { get; set; }
        public string ConditionDescription { get; set; }
        public string? ConditionImage { get; set; }
        [NotMapped]
        public IFormFile? ConditionPhoto { get; set; }
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
        public int LanguageGroup { get; set; }
    }
}
