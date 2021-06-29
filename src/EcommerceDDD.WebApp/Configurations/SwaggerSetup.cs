using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace EcommerceDDD.WebApp.Configurations
{
    public static class SwaggerSetup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwaggerSetup(this IServiceCollection services)
        {
            if (services == null) 
                throw new ArgumentNullException(nameof(services));

            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Ecommerce DDD",
                    Description = "Ecommerce DDD API Swagger",
                    Contact = new OpenApiContact { Name = "Felipe Alberto", Email = "fealberto@gmail.com" },
                    License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://github.com/falberthen/EcommerceDDD/blob/master/LICENSE") }
                });

                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });

            });
        }

        public static void UseSwaggerSetup(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce DDD");
            });
        }
    }
}
