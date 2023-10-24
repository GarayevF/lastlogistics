using System.ComponentModel.DataAnnotations.Schema;

namespace Logistics.Models
{
    public class Container : BaseEntity
    {
        public string? AboutTitle { get; set; }
        public string? AboutDescription { get; set; }
        public string? AboutImage { get; set; }
        [NotMapped]
        public IFormFile? AboutPhoto { get; set; }
        public string? FutureTitle { get; set; }
        public string? FutureDescription { get; set; }
        public string? FutureImage { get; set; }
        [NotMapped]
        public IFormFile? FuturePhoto { get; set; }
        public List<ContainerType>? ContainerTypes { get; set; }

    }
}
