namespace EcommerceDDD.CustomerManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/customers")]
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
    /// Get customer details using user's token
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = Roles.Customer, Policy = Policies.CanRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CustomerDetails>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDetailsByUserToken(CancellationToken cancellationToken) =>
        await Response(
            GetCustomerDetailsWithToken.Create(
                await _tokenRequester.GetUserTokenFromHttpContextAsync()),
                cancellationToken
        );

    /// <summary>
    /// Get customer event history
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId:guid}/history")]
    [Authorize(Roles = Roles.Customer, Policy = Policies.CanRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<CustomerEventHistory>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListHistory([FromRoute] Guid customerId,
        CancellationToken cancellationToken) =>
        await Response(
            GetCustomerEventHistory.Create(CustomerId.Of(customerId)),
            cancellationToken
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
            ), cancellationToken
        );

    /// <summary>
    /// Update customer's information
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut, Route("{customerId:guid}")]
    [Authorize(Roles = Roles.Customer, Policy = Policies.CanWrite)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateInformation([FromRoute] Guid customerId,
        [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken) =>
        await Response(
            UpdateCustomerInformation.Create(
                CustomerId.Of(customerId),
                request.Name,
                request.ShippingAddress,
                request.CreditLimit
            ), cancellationToken
        );

    /// <summary>
    /// Get customer details | M2M only
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("{customerId:guid}/details")]
    [Authorize(Policy = Policies.M2MAccess)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CustomerDetails>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDetailsByCustomerId([FromRoute] Guid customerId,
        CancellationToken cancellationToken) =>
        await Response(
            GetCustomerDetails.Create(CustomerId.Of(customerId)),
            cancellationToken
        );

    /// <summary>
    /// Get customer available credit limit | M2M only
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId:guid}/check-credit")]
    [Authorize(Policy = Policies.M2MAccess)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CreditLimitModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerCreditLimit([FromRoute] Guid customerId,
        CancellationToken cancellationToken) =>
        await Response(
            GetCreditLimit.Create(CustomerId.Of(customerId)),
            cancellationToken
        );
}