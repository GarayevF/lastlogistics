namespace Logistics.Models
{
    public class Priority : BaseEntity
    {
        public string  Description { get; set; }
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
        public int LanguageGroup { get; set; }
    }
}
