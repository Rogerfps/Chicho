using System.ComponentModel.DataAnnotations;

namespace CarGoCR.Models
{
    public class Paquete
    {
        public int Id { get; set; }

        [Required]
        public string Tracking { get; set; } = string.Empty;

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public decimal Peso { get; set; }

        public decimal ValorDeclarado { get; set; }

        public decimal CostoEnvio { get; set; }

        public DateTime FechaRecepcion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaEntrega { get; set; } 

        public string Estado { get; set; } = "Prealertado";

        public bool Pagado { get; set; }

        public string? Observaciones { get; set; }

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public int? TarifaId { get; set; }
        public Tarifa? Tarifa { get; set; }
    }
}

