using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Infrastructure.Services;
using System.Text.Json;

namespace Questao5.Application.Handlers
{
    public class CreateMovementHandler : IRequestHandler<CreateMovementRequest, CreateMovementResponse>
    {
        private readonly IDatabaseService _databaseService;

        public CreateMovementHandler(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<CreateMovementResponse> Handle(CreateMovementRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var idempotencyKey = await _databaseService.GetIdempotencyKey(request.Chaveidempotencia.ToString());
                if (idempotencyKey != null)
                {
                    return JsonSerializer.Deserialize<CreateMovementResponse>(idempotencyKey.Resultado);
                }

                var account = await _databaseService.GetAccountByNumero(request.Numero);
                if (account == null)
                {
                    return new CreateMovementResponse 
                    { 
                        Success = false,
                        ErrorType = "INVALID_ACCOUNT"
                    };
                }

                if (account.Ativo != 1)
                {
                    return new CreateMovementResponse 
                    { 
                        Success = false,
                        ErrorType = "INACTIVE_ACCOUNT"
                    };
                }

                if (request.Valor <= 0)
                {
                    return new CreateMovementResponse 
                    { 
                        Success = false,
                        ErrorType = "INVALID_VALUE"
                    };
                }

                var tipoMovimento = request.TipoMovimento?.ToUpper();
                if (tipoMovimento != "C" && tipoMovimento != "D")
                {
                    return new CreateMovementResponse 
                    { 
                        Success = false,
                        ErrorType = "INVALID_TYPE"
                    };
                }

                var movementId = await _databaseService.CreateMovement(request);
                
                return new CreateMovementResponse 
                { 
                    IdMovimento = movementId,
                    Numero = account.Numero,
                    Valor = request.Valor,
                    DataMovimento = DateTime.Now,
                    Success = true
                };
            }
            catch (Exception)
            {
                return new CreateMovementResponse 
                { 
                    Success = false,
                    ErrorType = "INTERNAL_ERROR"
                };
            }
        }
    }
}