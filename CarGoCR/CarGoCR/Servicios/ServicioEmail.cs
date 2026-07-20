using System.Net;
using System.Net.Mail;

namespace CarGoCR.Servicios
{
    public interface IServicioEmail
    {
        Task EnviarEmail(string emailReceptor, string tema, string cuerpo);
    }

    public class ServicioEmail : IServicioEmail
    {
        private readonly IConfiguration _configuration;

        public ServicioEmail(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarEmail(string emailReceptor,
                                      string tema,
                                      string cuerpo)
        {
            var emailEmisor = _configuration["CONFIGURACIONES_EMAIL:EMAIL"];
            var password = _configuration["CONFIGURACIONES_EMAIL:PASSWORD"];
            var host = _configuration["CONFIGURACIONES_EMAIL:HOST"];
            var puerto = _configuration.GetValue<int>("CONFIGURACIONES_EMAIL:PUERTO");

            using var smtp = new SmtpClient(host, puerto);

            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;

            smtp.Credentials = new NetworkCredential(
                emailEmisor,
                password);

            var mensaje = new MailMessage(
                emailEmisor!,
                emailReceptor,
                tema,
                cuerpo);

            mensaje.IsBodyHtml = true;

            await smtp.SendMailAsync(mensaje);
        }
    }
}
