using System.Threading.Tasks;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Services
{
    public interface IDatabaseService
    {
        Task<Account> GetAccountByNumero(int numeroContaCorrente);
        Task<Account> GetAccountById(string idContaCorrente);
        Task<string> CreateMovement(CreateMovementRequest request);
        Task<decimal> GetBalanceByNumero(int numeroContaCorrente);
        Task<decimal> GetBalance(string idContaCorrente);
        Task<Idempotency> GetIdempotencyKey(string chaveIdempotencia);
        Task SaveIdempotency(string chaveIdempotencia, string requisicao, string resultado);
    }
} 