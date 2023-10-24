namespace Logistics.Models
{
    public class OurCustomer : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
        public int LanguageGroup { get; set; }
    }
}
