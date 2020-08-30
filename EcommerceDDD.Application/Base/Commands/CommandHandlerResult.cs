using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Application.Base.Commands
{
    public class CommandHandlerResult
    {
        public ValidationResult ValidationResult { get; set; }
        public Guid Id { get; set; }
    }
}
