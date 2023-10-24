using Logistics.Models;

namespace Logistics.ViewModels
{
    public class IncotermsVM
    {
        public Incoterms? Incoterms { get; set; }
        public List<IncotermsSection>? IncotermsSection { get; set; }
    }
}
