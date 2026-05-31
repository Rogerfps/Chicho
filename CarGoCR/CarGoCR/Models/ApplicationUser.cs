using Microsoft.AspNetCore.Identity;

namespace CarGoCR.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
}