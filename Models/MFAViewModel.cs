using System.ComponentModel.DataAnnotations;

namespace identity.Models
{
    public class MFAViewModel
    {
        [Required]
        public string Token {get; set;}
        [Required]
        public string Code { get; set; }
    }
}