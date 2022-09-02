namespace EcommerceDDD.IntegrationServices.Products.Requests;

public record class GetProductsRequest(string CurrencyCode, Guid[] ProductIds);