using System.ComponentModel.DataAnnotations;

namespace Logistics.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Bu xana bosh ola bilmez")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Bu xana bosh ola bilmez")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsRemember { get; set; }

    }
}
