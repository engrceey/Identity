using System.ComponentModel.DataAnnotations;

namespace identity.Models
{
    public class SigninViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}