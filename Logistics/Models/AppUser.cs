using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Logistics.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsDeactive { get; set; }
    }
}
