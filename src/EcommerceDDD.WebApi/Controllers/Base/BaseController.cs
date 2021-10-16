using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EcommerceDDD.Application.Core.CQRS.QueryHandling;
using EcommerceDDD.Application.Core.CQRS.CommandHandling;

namespace EcommerceDDD.WebApi.Controllers.Base
{
    public class BaseController : Controller
    {
        public readonly IMediator Mediator;

        protected BaseController(IMediator mediator)
        {
            Mediator = mediator;
        }

        protected async new Task<IActionResult> Response<TResult>(Query<TResult> query)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var queryHandlerResult = await Mediator.Send(query);
                return queryHandlerResult.ValidationResult.IsValid ? OkActionResult(queryHandlerResult.Result)
                    : BadRequestActionResult(queryHandlerResult.ValidationResult.Errors);
            }
            catch (Exception e)
            {
                return BadRequestActionResult(e.Message);
            }            
        }

        protected async new Task<IActionResult> Response<TResult>(Command<TResult> command) 
            where TResult : struct
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var commandHandlerResult = await Mediator.Send(command);
                return commandHandlerResult.ValidationResult.IsValid ? OkActionResult(commandHandlerResult.Id)
                    : BadRequestActionResult(commandHandlerResult.ValidationResult.Errors);
            }
            catch (Exception e)
            {
                return BadRequestActionResult(e.Message);
            }
        }

        private IActionResult BadRequestActionResult(dynamic resultErrors)
        {
            return BadRequest(new
            {
                success = false,
                message = resultErrors
            });
        }

        private IActionResult OkActionResult(dynamic resultData)
        {
            return Ok(new
            {
                success = true,
                data = resultData
            });
        }
    }
}
