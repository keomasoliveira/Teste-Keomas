using Microsoft.AspNetCore.Mvc;
using MediatR;

using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using System.Threading.Tasks;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Commands.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace Infrastructure.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OperationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OperationsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("movimentar")]
        [SwaggerOperation(Summary = "Cria uma nova movimentação", Description = "Cria uma nova movimentação para uma conta corrente.  Em chaveidempotencia deve ser colocado no padrão system.guid xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx")]
        [ProducesResponseType(typeof(CreateMovementResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMovement([FromBody] CreateMovementRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        errorType = "INVALID_REQUEST" 
                    });
                }

                var response = await _mediator.Send(request);

                if (!response.Success)
                {
                    return BadRequest(new 
                    { 
                        success = response.Success, 
                        errorType = response.ErrorType 
                    });
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new 
                    { 
                        success = false, 
                        errorType = "INTERNAL_ERROR" 
                    });
            }
        }


        [HttpGet("saldo/{numeroContaCorrente}")]
        [SwaggerOperation(Summary = "Obtém o saldo de uma conta corrente", Description = "Retorna o saldo da conta corrente especificada.")]
        [ProducesResponseType(typeof(GetBalanceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GetBalanceResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBalance(int numeroContaCorrente)
        {
            try
            {
                var query = new GetBalanceRequest { Numero = numeroContaCorrente };
                var response = await _mediator.Send(query);

                if (!response.Success)
                    return BadRequest(new 
                    { 
                        success = response.Success, 
                        errorType = response.ErrorType 
                    });

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new 
                    { 
                        success = false, 
                        errorType = "INTERNAL_ERROR" 
                    });
            }
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public string Type { get; set; }

        public ErrorResponse(string type)
        {
            Type = type;
        }
    }
} 