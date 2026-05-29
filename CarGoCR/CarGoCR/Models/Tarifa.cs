using System.ComponentModel.DataAnnotations;

namespace CarGoCR.Models
{
    public class Tarifa
    {
        public int Id { get; set; }

        [Required]
        public string Categoria { get; set; } = string.Empty;

        [Required]
        public decimal PesoMinimo { get; set; }

        [Required]
        public decimal PesoMaximo { get; set; }

        [Required]
        public decimal PrecioNacional { get; set; }

        [Required]
        public decimal PrecioInternacional { get; set; }

        public bool RequiereCotizacion { get; set; } = false;

        public bool Activo { get; set; } = true;
    }
}
