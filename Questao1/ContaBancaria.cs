using System;
using System.Globalization;

namespace Questao1
{
    class ContaBancaria
    {
        public int Numero { get; private set; }
        public string Titular { get; set; }
        private double Saldo;

    
        public ContaBancaria(int numero, string titular)
        {
            Numero = numero;
            Titular = titular;
            Saldo = 0.0;
        }

        public ContaBancaria(int numero, string titular, double depositoInicial) : this(numero, titular)
        {
            Deposito(depositoInicial);
        }

    
        public void Deposito(double quantia)
        {
            if (quantia > 0)
            {
                Saldo += quantia;
            }
            else
            {
                throw new ArgumentException("A quantia para depósito deve ser maior que zero.");
            }
        }

        public void Saque(double quantia)
        {
            const double taxa = 3.50;
            if (quantia > 0)
            {
                Saldo -= (quantia + taxa);
            }
            else
            {
                throw new ArgumentException("A quantia para saque deve ser maior que zero.");
            }
        }

        public override string ToString()
        {
            return $"Conta {Numero}, Titular: {Titular}, Saldo: $ {Saldo.ToString("F2", CultureInfo.InvariantCulture)}";
        }
    }
}
