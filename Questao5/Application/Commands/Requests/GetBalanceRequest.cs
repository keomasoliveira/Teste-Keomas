using Questao5.Application.Commands.Responses;
using MediatR;

namespace Questao5.Application.Commands.Requests
{
    public class GetBalanceRequest : IRequest<GetBalanceResponse>
    {
        public int Numero { get; set; }
    }
} 