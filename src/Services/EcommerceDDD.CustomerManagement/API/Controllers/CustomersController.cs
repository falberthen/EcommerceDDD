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
	[MapToApiVersion(ApiVersions.V2)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
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
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanRead)]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CustomerDetails>))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
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
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanRead)]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<IEventHistory>>))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
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
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanWrite)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> UpdateInformation([FromBody] UpdateCustomerRequest request, 
		CancellationToken cancellationToken) =>
			await Response(
				UpdateCustomerInformation.Create(
					request.Name,
					request.ShippingAddress,
					request.CreditLimit
				), cancellationToken
			);

	/// <summary>
	/// Get customer details by CustomerId | M2M only
	/// </summary>
	/// <returns></returns>
	[HttpGet, Route("{customerId:guid}/details")]
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.M2MAccess)]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CustomerDetails>))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetDetailsByCustomerId([FromRoute] Guid customerId,
		CancellationToken cancellationToken) =>
			await Response(
				GetCustomerDetailsById.Create(CustomerId.Of(customerId)),
				cancellationToken
			);

	/// <summary>
	/// Get customer available credit limit | M2M only
	/// </summary>
	/// <param name="customerId"></param>
	/// <returns></returns>
	[HttpGet, Route("{customerId:guid}/credit")]
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.M2MAccess)]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CreditLimitModel>))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetCustomerCreditLimit([FromRoute] Guid customerId, 
		CancellationToken cancellationToken) =>
			await Response(
				GetCreditLimit.Create(CustomerId.Of(customerId)),
				cancellationToken
			);
}