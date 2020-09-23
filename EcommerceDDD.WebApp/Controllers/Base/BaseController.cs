using System;
using BuildingBlocks.CQRS.CommandHandling;
using BuildingBlocks.CQRS.QueryHandling;
using EcommerceDDD.Infrastructure.Identity.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceDDD.WebApp.Controllers.Base
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

        protected new IActionResult Response<TResult>(QueryHandlerResult<TResult> queryHandlerResult)
        {
            return queryHandlerResult.ValidationResult.IsValid ? OkActionResult(queryHandlerResult.Result) 
                : BadRequestActionResult(queryHandlerResult.ValidationResult.Errors);
        }

        protected new IActionResult Response<TResult>(CommandHandlerResult<TResult> commandHandlerResult) where TResult : struct
        {
            return commandHandlerResult.ValidationResult.IsValid ? OkActionResult(commandHandlerResult.Id)
                : BadRequestActionResult(commandHandlerResult.ValidationResult.Errors);
        }

        protected new IActionResult Response(CommandHandlerResult commandHandlerResult)
        {
            return commandHandlerResult.ValidationResult.IsValid ? OkActionResult(commandHandlerResult)
                : BadRequestActionResult(commandHandlerResult.ValidationResult.Errors);
        }

        private IActionResult BadRequestActionResult(dynamic resultErrors)
        {
            return BadRequest(new
            {
                success = false,
                errors = resultErrors
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
