using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TokenProject.Model
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(256)]
        public string FirstName { get; set; }
        [Required, MaxLength(256)]
        public string LastName { get; set; }
    }
}
