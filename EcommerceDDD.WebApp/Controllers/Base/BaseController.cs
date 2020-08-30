using System;
using System.Collections.Generic;
using System.Linq;
using EcommerceDDD.Application.Base.Commands;
using EcommerceDDD.Infrastructure.Identity.Helpers;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ontactBookCQRS.WebApp.Controllers.Base
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

        protected new IActionResult Response(CommandHandlerResult result)
        {
            if (!result.ValidationResult.Errors.Any())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = result.ValidationResult.Errors
            });
        }

        protected new IActionResult Response(object result)
        {
            return Ok(new
            {
                success = true,
                data = result
            });
        }
    }
}
