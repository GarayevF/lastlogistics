namespace Logistics.Models
{
    public class ServiceServiceSection
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public int ServiceSectionId { get; set; }
        public ServiceSection ServiceSection { get; set; }
    }
}
