namespace EcommerceDDD.ServiceClients.Configuration;

public class ServiceClientsOptions
{
	public const string SectionName = "Services";

	public string? IdentityServer { get; set; }
	public string? CustomerManagement { get; set; }
	public string? InventoryManagement { get; set; }
	public string? ProductCatalog { get; set; }
	public string? QuoteManagement { get; set; }
	public string? PaymentProcessing { get; set; }
	public string? ShipmentProcessing { get; set; }
	public string? SignalRClient { get; set; }
}
