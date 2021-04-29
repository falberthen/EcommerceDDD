using System;
using System.Threading.Tasks;
using BuildingBlocks.CQRS.CommandHandling;
using BuildingBlocks.CQRS.QueryHandling;
using EcommerceDDD.Infrastructure.Identity.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceDDD.WebApi.Controllers.Base
{
    public class BaseController : Controller
    {
        private readonly IUserProvider _userProvider;
        public readonly IMediator Mediator;

        protected Guid UserId
        {
            get { return _userProvider.GetUserId(); }
        }

        protected BaseController(IUserProvider userProvider, IMediator mediator)
        {
            _userProvider = userProvider;
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

        protected new IActionResult Response(Command commandHandlerResult)
        {
            try
            {
                if (!ModelState.IsValid)
                return BadRequest();

                return commandHandlerResult.ValidationResult.IsValid ? OkActionResult(commandHandlerResult)
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
