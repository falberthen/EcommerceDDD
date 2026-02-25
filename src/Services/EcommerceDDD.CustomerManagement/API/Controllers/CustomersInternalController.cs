namespace EcommerceDDD.CustomerManagement.API.Controllers;

[Authorize(Roles = Roles.M2MAccess)] // M2M only
[ApiController]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/internal/customers")]
public class CustomersInternalController(
	ICommandBus commandBus,
	IQueryBus queryBus
) : CustomControllerBase(commandBus, queryBus)
{
	/// <summary>
	/// Get customer details by CustomerId
	/// </summary>
	/// <returns></returns>
	[HttpGet, Route("{customerId:guid}/details")]
	[Authorize(Policy = Policies.CanRead)]
	[ProducesResponseType(typeof(CustomerDetails), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetDetailsByCustomerId([FromRoute] Guid customerId,
		CancellationToken cancellationToken) =>
			await Response(
				GetCustomerDetailsById.Create(CustomerId.Of(customerId)),
				cancellationToken
			);

	/// <summary>
	/// Get customer available credit limit
	/// </summary>
	/// <param name="customerId"></param>
	/// <returns></returns>
	[HttpGet, Route("{customerId:guid}/credit")]
	[Authorize(Policy = Policies.CanRead)]
	[ProducesResponseType(typeof(CreditLimitModel), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetCustomerCreditLimit([FromRoute] Guid customerId,
		CancellationToken cancellationToken) =>
			await Response(
				GetCreditLimit.Create(CustomerId.Of(customerId)),
				cancellationToken
			);
}
