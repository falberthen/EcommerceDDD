namespace EcommerceDDD.IntegrationServices.Products.Requests;

public record class ProductStockAvailabilityRequest(Guid[] ProductIds);