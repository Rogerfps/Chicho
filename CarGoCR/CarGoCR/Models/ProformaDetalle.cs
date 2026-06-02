namespace CarGoCR.Models
{
    public class ProformaDetalle
    {
        public int Id { get; set; }

        public int ProformaId { get; set; }
        public Proforma? Proforma { get; set; }

        public int PaqueteId { get; set; }
        public Paquete? Paquete { get; set; }

        public decimal Precio { get; set; }
    }
}
