using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models
{
    public class AppUser:IdentityUser
    {
        [Required]
        public string? FullName {  get; set; }
    }
}
