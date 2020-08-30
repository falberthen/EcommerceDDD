using System;
using AutoMapper;
using EcommerceDDD.Application.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDDD.WebApp.Configurations
{
    public static class AutoMapperSetup
    {
        public static void AddAutoMapperSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(typeof(RequestToCommandProfile));
        }
    }
}
