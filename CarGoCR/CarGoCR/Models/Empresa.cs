using System.ComponentModel.DataAnnotations;

namespace CarGoCR.Models
{
    public class Empresa
    {
        public int Id { get; set; }

        public string NombreEmpresa { get; set; } = "";

        public string? RazonSocial { get; set; }

        public string? CedulaJuridica { get; set; }

        public string? Industria { get; set; }

        public string? Descripcion { get; set; }

        public string? Correo { get; set; }

        public string? Telefono { get; set; }

        public string? SitioWeb { get; set; }

        public string? Facebook { get; set; }

        public string? Instagram { get; set; }

        public string? Twitter { get; set; }

        public string? MonedaPrincipal { get; set; }

        public string? MonedaSecundaria { get; set; }

        public decimal TipoCambio { get; set; }

        public decimal IVA { get; set; }

        public bool IncluirImpuestos { get; set; }

        public bool FacturacionElectronica { get; set; }

        public string? Logo { get; set; }
    }
}

