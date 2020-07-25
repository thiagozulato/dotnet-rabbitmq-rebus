using System.Threading.Tasks;
using Messages;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;

namespace Venda.Api
{
    public class VendaPagamentoEventHandler : IHandleMessages<PagamentoAprovadoEvent>
    {
        readonly ILogger<VendaPagamentoEventHandler> Log;
        readonly IBus _bus;
        public VendaPagamentoEventHandler(ILogger<VendaPagamentoEventHandler> log, IBus bus)
        {
            Log = log;
            _bus = bus;
        }
        public async Task Handle(PagamentoAprovadoEvent message)
        {
            Log.LogInformation("O Pagamento foi aprovado, atualizar banco de dados");

            await Task.Delay(5000);
        }
    }
}