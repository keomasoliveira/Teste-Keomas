using NSubstitute;
using Xunit;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Application.Handlers;
using Questao5.Infrastructure.Services;
using Questao5.Domain.Entities;

public class GetBalanceHandlerTests
{
    private readonly IDatabaseService _databaseService;
    private readonly GetBalanceHandler _handler;

    public GetBalanceHandlerTests()
    {
        _databaseService = Substitute.For<IDatabaseService>();
        _handler = new GetBalanceHandler(_databaseService);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsBalanceResponse()
    {
        var request = new GetBalanceRequest { Numero = 123 };
        var account = new Account
        {
            Numero = 123,
            Ativo = 1
        };

        _databaseService.GetAccountByNumero(request.Numero).Returns(account);
        _databaseService.GetBalanceByNumero(request.Numero).Returns(250.00m);

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal(123, response.Numero);
        Assert.Equal(250.00m, response.Saldo);
    }

    [Fact]
    public async Task Handle_InvalidAccount_ReturnsErrorResponse()
    {
        var request = new GetBalanceRequest { Numero = 123 };

        _databaseService.GetAccountByNumero(request.Numero).Returns((Account)null);

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT", response.ErrorType);
    }

    [Fact]
    public async Task Handle_InactiveAccount_ReturnsErrorResponse()
    {
        var request = new GetBalanceRequest { Numero = 741 };

        var account = new Account
        {
            Numero = 741,
        };

        _databaseService.GetAccountByNumero(request.Numero).Returns(account);

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INACTIVE_ACCOUNT", response.ErrorType);
    }
} 