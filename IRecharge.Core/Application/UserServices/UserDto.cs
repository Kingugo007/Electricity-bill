using System.ComponentModel.DataAnnotations;

namespace IRecharge.Core.Application.UserServices
{

    public class CreateUserDto
    {
        [Required]
        public string PhoneNumber {  get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }    
    }
}
