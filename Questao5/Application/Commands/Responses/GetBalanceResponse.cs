namespace Questao5.Application.Commands.Responses
{
    public class GetBalanceResponse
    {
        public int Numero { get; set; }
        public string Nome { get; set; }
        public DateTime DataResposta { get; set; }
        public decimal Saldo { get; set; }
        public bool Success { get; set; }
        public string ErrorType { get; set; }
    }
} 