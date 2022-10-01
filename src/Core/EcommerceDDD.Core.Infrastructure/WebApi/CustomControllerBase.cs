using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Core.Infrastructure.WebApi;

public class CustomControllerBase : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomControllerBase() {}

    protected CustomControllerBase(IMediator mediator)
    {
        _mediator = mediator;
    }

    protected async new Task<IActionResult> Response<TResult>(IQuery<TResult> query)
    {
        TResult result = default!;

        try
        {
            if (!ModelState.IsValid)
                return BadRequest();

            result = await _mediator.Send(query);
        }
        catch (Exception e)
        {
            return BadRequestActionResult(e.Message);
        }

        return Ok(new
        {
            data = result,
            success = true
        });
    }

    protected async new Task<IActionResult> Response(ICommand command, object? data = null)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _mediator.Send(command);
        }
        catch (Exception e)
        {
            return BadRequestActionResult(e.Message);
        }

        return Ok(new
        {
            data = data,
            success = true
        });
    }

    private IActionResult BadRequestActionResult(dynamic resultErrors)
    {
        return BadRequest(new
        {
            success = false,
            message = resultErrors
        });
    }
}