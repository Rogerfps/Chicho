using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CarGoCR.Servicios;

namespace CarGoCR.Controllers
{
    [Route("api/emails")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly IServicioEmail servicioEmail;
        public EmailsController(IServicioEmail servicioEmail)
        {
            this.servicioEmail = servicioEmail;
        }

        [HttpPost]
        public async Task<ActionResult> Enviar(string email, string tema, string cuerpo)
        {
            await servicioEmail.EnviarEmail(email, tema, cuerpo);
            return Ok();
        }
    }
}
