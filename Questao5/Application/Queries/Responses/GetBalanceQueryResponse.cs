using System;

namespace Questao5.Application.Queries.Responses
{
    public class GetBalanceQueryResponse
    {
        public int Numero { get; set; }
        public string Nome { get; set; }
        public DateTime DataResposta { get; set; }
        public decimal Saldo { get; set; }
        public string Message { get; set; }
        public string ErrorType { get; set; }
    }
} 