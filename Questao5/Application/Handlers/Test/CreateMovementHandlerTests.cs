using NSubstitute;
using Xunit;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Handlers;
using Questao5.Infrastructure.Services;
using Questao5.Domain.Entities;

public class CreateMovementHandlerTests
{
    private readonly IDatabaseService _databaseService;
    private readonly CreateMovementHandler _handler;

    public CreateMovementHandlerTests()
    {
        _databaseService = Substitute.For<IDatabaseService>();
        _handler = new CreateMovementHandler(_databaseService);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        var request = new CreateMovementRequest
        {
            Chaveidempotencia = Guid.NewGuid(),
            Numero = 123,
            Valor = 100.00m,
            TipoMovimento = "C"
        };

        var account = new Account
        {
         IdContaCorrente = "1",
            Numero = 123,
            Nome = "Test Account",
            Ativo = 1 // Conta ativa
        };

        _databaseService.GetAccountByNumero(request.Numero).Returns(account);
        _databaseService.CreateMovement(request).Returns("movement-id");

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal("movement-id", response.IdMovimento);
        Assert.Equal(123, response.Numero);
        Assert.Equal(100.00m, response.Valor);
    }

    [Fact]
    public async Task Handle_InvalidAccount_ReturnsErrorResponse()
    {
        var request = new CreateMovementRequest
        {
            Chaveidempotencia = Guid.NewGuid(),
            Numero = 123,
            Valor = 100.00m,
            TipoMovimento = "C"
        };

        _databaseService.GetAccountByNumero(request.Numero).Returns((Account)null);

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT", response.ErrorType);
    }

    [Fact]
    public async Task Handle_InactiveAccount_ReturnsErrorResponse()
    {
        var request = new CreateMovementRequest
        {
            Chaveidempotencia = Guid.NewGuid(),
            // CONTA INATIVA
            Numero = 741,
            Valor = 100.00m,
            TipoMovimento = "C"
        };

        var account = new Account
        {
            Numero = 741,
            Nome = "Test Account",
            Ativo = 0
        };

        _databaseService.GetAccountByNumero(request.Numero).Returns(account);

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INACTIVE_ACCOUNT", response.ErrorType);
    }
} 