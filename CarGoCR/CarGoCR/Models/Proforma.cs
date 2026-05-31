using System.ComponentModel.DataAnnotations;

namespace CarGoCR.Models
{
    public class Proforma
    {
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        public Cliente? Cliente { get; set; }

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public decimal PesoEstimado { get; set; }

        public decimal ValorDeclarado { get; set; }

        public decimal CostoEstimado { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public string Estado { get; set; } = "Pendiente";

        public string? Observaciones { get; set; }

        public int? TarifaId { get; set; }

        public Tarifa? Tarifa { get; set; }
    }
}
