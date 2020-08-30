using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Application.Base.Queries
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {

    }
}
