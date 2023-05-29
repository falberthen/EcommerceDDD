namespace EcommerceDDD.Customers.API.Controllers;

[Authorize]
[Route("api/customers")]
[ApiController]
public class CustomersController : CustomControllerBase
{
    private ITokenRequester _tokenRequester { get; set; }

    public CustomersController(
        ITokenRequester tokenRequester,
        ICommandBus commandBus, 
        IQueryBus queryBus)
        : base(commandBus, queryBus) 
    {
        _tokenRequester = tokenRequester;
    }

    /// <summary>
    /// Get customer details
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = PolicyBuilder.CustomerRole, Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CustomerDetails>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDetails() =>
        await Response(
            GetCustomerDetails.Create(await _tokenRequester.GetUserTokenFromHttpContextAsync())
        );

    /// <summary>
    /// Get customer available credit limit
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId:guid}/credit")]
    [Authorize(Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CreditLimitModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerCreditLimit([FromRoute] Guid customerId) =>
        await Response(
            GetCreditLimit.Create(CustomerId.Of(customerId))
        );

    /// <summary>
    /// Get customer event history
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId::guid}/history")]
    [Authorize(Roles = PolicyBuilder.CustomerRole, Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<CustomerEventHistory>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListHistory([FromRoute] Guid customerId) =>
        await Response(
            GetCustomerEventHistory.Create(CustomerId.Of(customerId))
        );    

    /// <summary>
    /// Register a new customer
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCustomerRequest request) =>
        await Response(
            RegisterCustomer.Create(
                request.Email,
                request.Password,
                request.PasswordConfirm,
                request.Name,
                request.ShippingAddress,
                request.CreditLimit
            )
        );

    /// <summary>
    /// Update customer's information
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut, Route("{customerId:guid}")]
    [Authorize(Roles = PolicyBuilder.CustomerRole, Policy = PolicyBuilder.WritePolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateInformation([FromRoute] Guid customerId, [FromBody] UpdateCustomerRequest request) =>    
        await Response(
            UpdateCustomerInformation.Create(
                CustomerId.Of(customerId),
                request.Name,
                request.ShippingAddress,
                request.CreditLimit
            )
        );    
}