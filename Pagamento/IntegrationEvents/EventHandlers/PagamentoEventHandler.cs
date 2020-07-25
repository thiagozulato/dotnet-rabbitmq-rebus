using System.Threading.Tasks;
using Messages;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;

namespace Pagamento.Api
{
    public class PagamentoEventHandler : IHandleMessages<PagamentoIniciadoEvent>
    {
        readonly ILogger<PagamentoEventHandler> Log;
        readonly IBus _bus;
        public PagamentoEventHandler(ILogger<PagamentoEventHandler> log, IBus bus)
        {
            Log = log;
            _bus = bus;
        }
        public async Task Handle(PagamentoIniciadoEvent message)
        {
            Log.LogInformation("O evento PagamentoIniciadoEvent foi recebido");

            Log.LogInformation("Pagamento Processado e aprovado");

            await _bus.Publish(new PagamentoAprovadoEvent(message.IdPedido, message.IdUsuario));
        }
    }
}