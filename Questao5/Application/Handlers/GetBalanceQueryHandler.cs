using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Infrastructure.Services;


namespace Application.Handlers
{
    public class GetBalanceQueryHandler : IRequestHandler<GetBalanceQuery, GetBalanceQueryResponse>
    {
        private readonly IDatabaseService _databaseService;

        public GetBalanceQueryHandler(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<GetBalanceQueryResponse> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
        {
            var account = await _databaseService.GetAccountById(request.IdContaCorrente);
            if (account == null) 
                return new GetBalanceQueryResponse 
                { 
                    Numero = -1,
                    Message = "Conta corrente não encontrada",
                    ErrorType = "INVALID_ACCOUNT"
                };
            
            if (account.Ativo == 0)
                return new GetBalanceQueryResponse 
                { 
                    Numero = -2,
                    Message = "Conta corrente inativa",
                    ErrorType = "INACTIVE_ACCOUNT"
                };

            var saldo = await _databaseService.GetBalance(request.IdContaCorrente);
            
            return new GetBalanceQueryResponse
            {
                Numero = account.Numero,
                Nome = account.Nome,
                DataResposta = DateTime.UtcNow,
                Saldo = saldo
            };
        }
    }
} 