using System.ComponentModel.DataAnnotations;

namespace identity.Models
{
    public class MNFACheckViewModel
    {
        [Required]
        public string Code { get; set; }
    }
}