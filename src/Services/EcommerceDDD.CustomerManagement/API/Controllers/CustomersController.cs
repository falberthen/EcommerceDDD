namespace EcommerceDDD.CustomerManagement.API.Controllers;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/customers")]
public class CustomersController(
	ICommandBus commandBus,
	IQueryBus queryBus
) : CustomControllerBase(commandBus, queryBus)
{
	/// <summary>
	/// Register a new customer
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	[HttpPost]
	[AllowAnonymous]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> Register([FromBody] RegisterCustomerRequest request,
		CancellationToken cancellationToken) =>
			await Response(
				RegisterCustomer.Create(
					request.Email,
					request.Password,
					request.PasswordConfirm,
					request.Name,
					request.ShippingAddress,
					request.CreditLimit
				),
				cancellationToken
			);

	/// <summary>
	/// Get customer details for authenticated users with identity
	/// </summary>
	/// <returns></returns>
	[HttpGet, Route("details")]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanRead)]
	[ProducesResponseType(typeof(CustomerDetails), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetUserDetails(CancellationToken cancellationToken) =>
		await Response(
			GetCustomerDetails.Create(),
			cancellationToken
		);

	/// <summary>
	/// Get customer event history
	/// </summary>
	/// <returns></returns>
	[HttpGet, Route("history")]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanRead)]
	[ProducesResponseType(typeof(IReadOnlyList<CustomerEventHistory>), StatusCodes.Status200OK)]
	public async Task<IActionResult> ListHistory(CancellationToken cancellationToken) =>
		await Response(
			GetCustomerEventHistory.Create(),
			cancellationToken
		);

	/// <summary>
	/// Update customer's information
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	[HttpPut, Route("update")]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanWrite)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> UpdateInformation([FromBody] UpdateCustomerRequest request,
		CancellationToken cancellationToken) =>
			await Response(
				UpdateCustomerInformation.Create(
					request.Name,
					request.ShippingAddress,
					request.CreditLimit
				), cancellationToken
			);

}
