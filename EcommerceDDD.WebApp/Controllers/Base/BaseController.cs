using System;
using System.Linq;
using BuildingBlocks.CQRS.Core;
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

        protected new IActionResult Response(dynamic handlerResult)
        {
            var resultData = new Object();
            var dynamicResult = (dynamic)handlerResult;

            if (handlerResult.ValidationResult.Errors.Count == 0)
            {                
                switch (handlerResult.GetType().Name)
                {
                    case "QueryHandlerResult`1":
                        resultData = dynamicResult.Result;
                        break;
                    case "CommandHandlerResult`1":
                        resultData = dynamicResult.Id;
                        break;
                    default:
                        break;
                }

                return Ok(new
                {
                    success = true,
                    data = resultData
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = handlerResult.ValidationResult.Errors
            });
        }
    }
}
