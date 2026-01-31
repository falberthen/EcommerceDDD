namespace EcommerceDDD.Core.Infrastructure.WebApi;

/// <summary>
/// Operation filter that adds Bearer security requirement only to endpoints requiring authorization
/// </summary>
public class SecurityRequirementsOperationFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		// Check if endpoint has AllowAnonymous attribute
		var hasAllowAnonymous = context.ApiDescription.ActionDescriptor.EndpointMetadata
			.Any(em => em is AllowAnonymousAttribute);

		// Check if endpoint requires authorization
		var hasAuthorize = context.ApiDescription.ActionDescriptor.EndpointMetadata
			.Any(em => em is AuthorizeAttribute);

		// Only add security if endpoint requires authorization and doesn't allow anonymous
		if (hasAuthorize && !hasAllowAnonymous)
		{
			// Use OpenApiSecuritySchemeReference for Microsoft.OpenApi v2.x
			var securitySchemeReference = new OpenApiSecuritySchemeReference("Bearer", context.Document);

			operation.Security = new List<OpenApiSecurityRequirement>
			{
				new OpenApiSecurityRequirement
				{
					[securitySchemeReference] = new List<string>()
				}
			};
		}
	}
}
