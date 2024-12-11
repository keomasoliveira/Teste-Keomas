using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Infrastructure.Services;

namespace Application.Handlers
{
    public class GetBalanceHandler : IRequestHandler<GetBalanceRequest, GetBalanceResponse>
    {
        private readonly IDatabaseService _databaseService;

        public GetBalanceHandler(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<GetBalanceResponse> Handle(GetBalanceRequest request, CancellationToken cancellationToken)
        {
            var account = await _databaseService.GetAccountByNumero(request.Numero);
            if (account == null) 
                return new GetBalanceResponse 
                { 
                    Success = false,
                    ErrorType = "INVALID_ACCOUNT"
                };
            
            if (account.Ativo == 0) 
                return new GetBalanceResponse 
                { 
                    Success = false,
                    ErrorType = "INACTIVE_ACCOUNT"
                };

            var saldo = await _databaseService.GetBalanceByNumero(request.Numero);
            
            return new GetBalanceResponse
            {
                Numero = account.Numero,
                Nome = account.Nome,
                DataResposta = DateTime.UtcNow,
                Saldo = saldo,
                Success = true
            };
        }
    }
} 