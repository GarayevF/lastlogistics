using Logistics.Models;

namespace Logistics.ViewModels
{
    public class AboutVM
    {
        public About? About { get; set; }
        public Priority? Priority { get; set; }
        public List<PriorityCard>? PriorityCards { get; set; }
    }
}
