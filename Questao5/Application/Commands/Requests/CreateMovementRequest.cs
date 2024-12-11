using Questao5.Application.Commands.Responses;
using MediatR;

namespace Questao5.Application.Commands.Requests
{
    public class CreateMovementRequest : IRequest<CreateMovementResponse>
    {
        public Guid Chaveidempotencia { get; set; }
        public int Numero { get; set; }
        public decimal Valor { get; set; }
        public string TipoMovimento { get; set; }
    }
} 