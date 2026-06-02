using System.ComponentModel.DataAnnotations;

namespace CarGoCR.Models
{
    public class Proforma
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public decimal Total { get; set; }

        public bool Pagada { get; set; }

        public string Estado { get; set; } = "Pendiente";

        public string? Observaciones { get; set; }

        public ICollection<ProformaDetalle> Detalles { get; set; }
            = new List<ProformaDetalle>();
    }
}
