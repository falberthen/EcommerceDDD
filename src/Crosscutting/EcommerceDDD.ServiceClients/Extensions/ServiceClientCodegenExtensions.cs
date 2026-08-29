namespace EcommerceDDD.ServiceClients.Extensions;

public static class ServiceClientCodegenExtensions
{
	/// <summary>
	/// The Kiota clients register through AddHttpClient, an opaque factory that Wolverine 6's
	/// code generation cannot inline-construct. Route exactly these types through the service locator;
	/// anything else that needs it still throws under the default ServiceLocationPolicy.NotAllowed.
	/// </summary>
	public static WolverineOptions UseServiceClientServiceLocation(this WolverineOptions options)
	{
		options.CodeGeneration.AlwaysUseServiceLocationFor<IdentityServerClient>();
		options.CodeGeneration.AlwaysUseServiceLocationFor<CustomerManagementClient>();
		options.CodeGeneration.AlwaysUseServiceLocationFor<InventoryManagementClient>();
		options.CodeGeneration.AlwaysUseServiceLocationFor<ProductCatalogClient>();
		options.CodeGeneration.AlwaysUseServiceLocationFor<QuoteManagementClient>();
		options.CodeGeneration.AlwaysUseServiceLocationFor<PaymentProcessingClient>();
		options.CodeGeneration.AlwaysUseServiceLocationFor<ShipmentProcessingClient>();
		options.CodeGeneration.AlwaysUseServiceLocationFor<SignalRClient>();
		return options;
	}
}
