namespace EcommerceDDD.Customers.API.Controllers;

[Authorize]
[Route("api/customers")]
[ApiController]
public class CustomersController : CustomControllerBase
{
    public CustomersController(
        ICommandBus commandBus, 
        IQueryBus queryBus)
        : base(commandBus, queryBus) { }

    /// <summary>
    /// Get customer details
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.ReadPolicy)]    
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDetails()
    {
        var userAccessToken = await HttpContext.GetTokenAsync("access_token");
        var query = GetCustomerDetails.Create(userAccessToken!);
        return await Response(query);
    }

    /// <summary>
    /// Get customer available credit limit
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId}/credit")]
    [Authorize(Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerCreditLimit([FromRoute] Guid customerId)
    {
        var query = GetCreditLimit.Create(CustomerId.Of(customerId));
        return await Response(query);
    }

    /// <summary>
    /// Get customer event history
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId}/history")]
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListHistory([FromRoute] Guid customerId)
    {
        var query = GetCustomerEventHistory.Create(CustomerId.Of(customerId));
        return await Response(query);
    }

    /// <summary>
    /// Register a new customer
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCustomerRequest request)
    {
        var command = RegisterCustomer.Create(
            request.Email,
            request.Password,
            request.PasswordConfirm,
            request.Name,
            request.ShippingAddress,
            request.CreditLimit);

        return await Response(command);
    }

    /// <summary>
    /// Update customer's information
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut, Route("{customerId}")]
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.WritePolicy)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateInformation([FromRoute] Guid customerId, [FromBody] UpdateCustomerRequest request)
    {
        var command = UpdateCustomerInformation.Create(
            CustomerId.Of(customerId),
            request.Name,
            request.ShippingAddress,
            request.CreditLimit);

        return await Response(command);
    }
}