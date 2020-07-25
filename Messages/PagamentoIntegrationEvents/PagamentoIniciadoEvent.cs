using System;

namespace Messages
{
    public class PagamentoIniciadoEvent
    {
        public Guid Id { get; }
        public Guid IdPedido { get; }
        public Guid IdUsuario { get; }
        public string NumeroCartao { get; }

        public PagamentoIniciadoEvent(Guid idPedido, Guid idUsuario, string numeroCartao)
        {
            Id = Guid.NewGuid();
            IdPedido = idPedido;
            IdUsuario = idUsuario;
            NumeroCartao = numeroCartao;
        }
    }
}