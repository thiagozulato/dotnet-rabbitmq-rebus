using System;
using System.Threading.Tasks;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Rebus.Bus;

namespace Venda.Api
{
    [ApiController]
    [Route("api/v1/vendas")]
    public class VendaController : ControllerBase
    {
        readonly IBus _bus;

        public VendaController(IBus bus)
        {
            _bus = bus;
        }

        [HttpPost]
        public async Task<IActionResult> ConcluirVenda()
        {
            await _bus.Publish(new PagamentoIniciadoEvent(Guid.NewGuid(), Guid.NewGuid(), "5143.3465.2432.1999"));

            return Ok(new
            {
                Processado = "ok"
            });
        }
    }
}