using System.ComponentModel.DataAnnotations;

namespace Logistics.Models
{
    public class Statistics : BaseEntity
    {
        public string Title { get; set; }
        public int  Value { get; set; }
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
        public int LanguageGroup { get; set; }
    }
}
