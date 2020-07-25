using System;

namespace Messages
{
    public class PagamentoAprovadoEvent
    {
        public Guid Id { get; }
        public Guid IdPedido { get; }
        public Guid IdUsuario { get; }

        public PagamentoAprovadoEvent(Guid idPedido, Guid idUsuario)
        {
            Id = Guid.NewGuid();
            IdPedido = idPedido;
            IdUsuario = idUsuario;
        }
    }
}