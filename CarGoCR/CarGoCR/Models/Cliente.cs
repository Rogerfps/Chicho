using System.ComponentModel.DataAnnotations;

namespace CarGoCR.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}


