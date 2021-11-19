using EcommerceDDD.Application.Core.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDDD.WebApi.Configurations;

public static class AutoMapperSetup
{
    public static void AddAutoMapperSetup(this IServiceCollection services)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));

        services.AddAutoMapper(typeof(RequestToCommandProfile));
    }
}