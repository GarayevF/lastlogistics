namespace Logistics.Models
{
    public class StringResource:BaseEntity
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int? LanguageId { get; set; }
        public virtual Language Language { get; set; }
    }
}
