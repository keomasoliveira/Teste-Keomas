namespace Questao5.Application.Commands.Responses
{
    public class CreateMovementResponse
    {
        public string IdMovimento { get; set; }
        public int Numero { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataMovimento { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorType { get; set; }
    }
} 