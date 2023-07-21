using System.ComponentModel.DataAnnotations;

namespace Web.DatingApp.API.Web.DatingApp.Dtos
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(10)]
        public string Password { get; set; }
    }
}
