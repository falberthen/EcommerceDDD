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
            if (!queryHandlerResult.ValidationResult.IsValid)
                BadRequestActionResult(queryHandlerResult.ValidationResult.Errors);

            return OkActionResult(queryHandlerResult.Result);
        }

        protected new IActionResult Response<TResult>(CommandHandlerResult<TResult> commandHandlerResult) where TResult : struct
        {
            if (!commandHandlerResult.ValidationResult.IsValid)
                BadRequestActionResult(commandHandlerResult.ValidationResult.Errors);

            return OkActionResult(commandHandlerResult.Id);
        }

        protected new IActionResult Response(CommandHandlerResult commandHandlerResult)
        {
            if (!commandHandlerResult.ValidationResult.IsValid)
                BadRequestActionResult(commandHandlerResult.ValidationResult.Errors);

            return OkActionResult(commandHandlerResult);
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
