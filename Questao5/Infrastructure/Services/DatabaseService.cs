using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly DatabaseConfig _databaseConfig;

        public DatabaseService(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task<string> CreateMovement(CreateMovementRequest request)
        {
            var idMovimento = Guid.NewGuid().ToString();
            using var connection = new SqliteConnection(_databaseConfig.Name);

            var account = await connection.QueryFirstOrDefaultAsync<Account>(
                @"SELECT 
                    idcontacorrente as IdContaCorrente, 
                    numero as Numero, 
                    nome as Nome, 
                    ativo as Ativo 
                FROM contacorrente 
                WHERE numero = @Numero",
                new { Numero = request.Numero });

            if (account == null)
                throw new Exception("INVALID_ACCOUNT");

            if (account.Ativo != 1)
                throw new Exception("INACTIVE_ACCOUNT");

            await connection.ExecuteAsync(
                @"INSERT INTO movimento 
                  (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) 
                  VALUES 
                  (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)",
                new
                {
                    IdMovimento = idMovimento,
                    IdContaCorrente = account.IdContaCorrente,
                    DataMovimento = DateTime.Now.ToString("dd/MM/yyyy"),
                    TipoMovimento = request.TipoMovimento.ToUpper(),
                    Valor = request.Valor
                });

            return idMovimento;
        }

        public async Task<decimal> GetBalanceByNumero(int numeroContaCorrente)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            
            var account = await connection.QueryFirstOrDefaultAsync<Account>(
                @"SELECT idcontacorrente as IdContaCorrente
                FROM contacorrente 
                WHERE numero = @Numero",
                new { Numero = numeroContaCorrente });

            if (account == null)
                throw new Exception("INVALID_ACCOUNT");

            var saldo = await connection.QueryFirstOrDefaultAsync<decimal>(
                @"SELECT COALESCE(
                    (SELECT SUM(valor) FROM movimento WHERE idcontacorrente = @IdContaCorrente AND tipomovimento = 'C'), 
                    0) -
                  COALESCE(
                    (SELECT SUM(valor) FROM movimento WHERE idcontacorrente = @IdContaCorrente AND tipomovimento = 'D'),
                    0) as saldo",
                new { IdContaCorrente = account.IdContaCorrente });

            return saldo;
        }

        public async Task<Account> GetAccountByNumero(int numeroContaCorrente)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            
            var sql = @"SELECT 
                idcontacorrente as IdContaCorrente, 
                numero as Numero, 
                nome as Nome, 
                ativo as Ativo 
            FROM contacorrente 
            WHERE numero = @Numero";

   
            var account = await connection.QueryFirstOrDefaultAsync<Account>(
                sql,
                new { Numero = numeroContaCorrente });

         
            return account;
        }

        public async Task<Idempotency> GetIdempotencyKey(string chaveIdempotencia)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            return await connection.QueryFirstOrDefaultAsync<Idempotency>(
                @"SELECT 
                    chave_idempotencia as ChaveIdempotencia, 
                    requisicao as Requisicao, 
                    resultado as Resultado 
                FROM idempotencia 
                WHERE chave_idempotencia = @ChaveIdempotencia",
                new { ChaveIdempotencia = chaveIdempotencia });
        }

        public async Task SaveIdempotency(string chaveIdempotencia, string requisicao, string resultado)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            await connection.ExecuteAsync(
                @"INSERT INTO idempotencia 
                  (chave_idempotencia, requisicao, resultado) 
                  VALUES 
                  (@ChaveIdempotencia, @Requisicao, @Resultado)",
                new 
                { 
                    ChaveIdempotencia = chaveIdempotencia, 
                    Requisicao = requisicao, 
                    Resultado = resultado 
                });
        }

        public async Task<Account> GetAccountById(string idContaCorrente)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            return await connection.QueryFirstOrDefaultAsync<Account>(
                @"SELECT 
                    idcontacorrente as IdContaCorrente, 
                    numero as Numero, 
                    nome as Nome, 
                    ativo as Ativo 
                FROM contacorrente 
                WHERE idcontacorrente = @IdContaCorrente",
                new { IdContaCorrente = idContaCorrente });
        }

        public async Task<decimal> GetBalance(string idContaCorrente)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            
            var account = await connection.QueryFirstOrDefaultAsync<Account>(
                @"SELECT idcontacorrente as IdContaCorrente
                FROM contacorrente 
                WHERE idcontacorrente = @IdContaCorrente",
                new { IdContaCorrente = idContaCorrente });

            if (account == null)
                throw new Exception("INVALID_ACCOUNT");

            var saldo = await connection.QueryFirstOrDefaultAsync<decimal>(
                @"SELECT COALESCE(
                    (SELECT SUM(valor) FROM movimento WHERE idcontacorrente = @IdContaCorrente AND tipomovimento = 'C'), 
                    0) -
                  COALESCE(
                    (SELECT SUM(valor) FROM movimento WHERE idcontacorrente = @IdContaCorrente AND tipomovimento = 'D'),
                    0) as saldo",
                new { IdContaCorrente = account.IdContaCorrente });

            return saldo;
        }
    }
} 