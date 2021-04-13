using System;
using System.Linq;
using BuildingBlocks.CQRS.CommandHandling;
using BuildingBlocks.CQRS.Core;
using BuildingBlocks.CQRS.QueryHandling;
using EcommerceDDD.Infrastructure.Identity.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceDDD.WebApp.Controllers.Base
{
    public class BaseController : Controller
    {
        private readonly IUserProvider _userProvider;

        protected Guid UserId
        {
            get { return _userProvider.GetUserId();}
        }

        protected BaseController(IUserProvider userProvider)
        {
            _userProvider = userProvider;
        }

        protected new IActionResult Response<TResult>(QueryHandlerResult<TResult> queryHandlerResult)
        {
            var isValid = queryHandlerResult.ValidationResult.IsValid;

            if (!isValid)
            {
                return BadRequest(new
                {
                    success = isValid,
                    errors = queryHandlerResult.ValidationResult.Errors
                });
            }

            return Ok(new
            {
                success = isValid,
                data = queryHandlerResult.Result
            });
        }

        protected new IActionResult Response<TResult>(CommandHandlerResult<TResult> commandHandlerResult) where TResult : struct
        {
            var isValid = commandHandlerResult.ValidationResult.IsValid;

            if (!isValid)
            {
                return BadRequest(new
                {
                    success = isValid,
                    errors = commandHandlerResult.ValidationResult.Errors
                });
            }

            return Ok(new
            {
                success = isValid,
                data = commandHandlerResult.Id
            });
        }
    }
}
